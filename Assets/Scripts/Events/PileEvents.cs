using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public class DrawCardEvent : BaseCardEvent
    {
        private PlayerType PlayerType { get; set; }

        public DrawCardEvent(PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public override void Process()
        {
            var player = MainController.GetPlayer(PlayerType);

            var card = player.TakeFromDeck();
            player.AddToHand(card);

            MainController.AddEvent(new DrawCardUIEvent(card));
        }
    }

    public class ReturnToDeckEvent : BaseCardEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process()
        {
            var player = MainController.GetPlayer(Card.Owner);

            player.RemoveFromHand(Card);
            player.AddToDeck(Card);

            MainController.AddEvent(new ReturnToDeckUIEvent(Card));
        }
    }

    public class ReturnToHandEvent : BaseCardEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToHandEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process()
        {
            var player = MainController.GetPlayer(Card.Owner);

            player.RemoveFromField(Card);
            player.AddToHand(Card);

            MainController.AddEvent(new AddToHandUIEvent(Card));
        }
    }


    public class EnterFieldEvent : BaseCardEvent
    {
        public BaseCard Card { get; set; }

        public EnterFieldEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process()
        {
            var player = MainController.GetPlayer(Card.Owner);

            if (Card.Area == Area.Hand)
                player.RemoveFromHand(Card);

            if (Card.Type != CardType.Action)
            {
                player.AddToField(Card);
            }
            else
            {
                MainController.AddEvent(new DestroyCardEvent(Card));
            }

            foreach (var baseEvent in Card.CardEvents)
            {
                MainController.AddEvent(baseEvent);
            }
        }
    }

    public class DestroyCardEvent : BaseCardEvent
    {
        public BaseCard Card { get; set; }

        public DestroyCardEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process()
        {
            // stop duplicate destroys - a better way to do this?
            if (Card.Area != Area.None && Area.Pile.HasFlag(Card.Area))
                return;

            var player = MainController.GetPlayer(Card.Owner);
            var cardArea = Card.Area;

            if (cardArea == Area.Hand)
                player.RemoveFromHand(Card);

            if (cardArea == Area.Field)
                player.RemoveFromField(Card);

            if (cardArea == Area.Deck)
                player.RemoveFromDeck(Card);

            if (!Card.IsSummoned)
            {
                player.AddToDestroyed(Card);
            }

            if (Area.PlayArea.HasFlag(cardArea))
                MainController.AddEvent(new DestroyCardUIEvent(Card));
        }
    }

    public class DestroyCreatureByDamageEvent : BaseCardEvent
    {
        public CreatureCard Card { get; set; }

        public DestroyCreatureByDamageEvent(CreatureCard card)
        {
            Card = card;
        }

        public override void Process()
        {
            // Check card hasn't now got more defence
            if (Card.Defence > 0)
                return;

            MainController.AddEvent(new DestroyCardEvent(Card));
        }
    }

    public class EliminateCardEvent : BaseCardEvent
    {
        private BaseCard Card { get; set; }
        private bool CreateUIEvent { get; set; }

        public EliminateCardEvent(BaseCard card, bool createUIEvent = true)
        {
            Card = card;
            CreateUIEvent = createUIEvent;
        }

        public override void Process()
        {
            var player = MainController.GetPlayer(Card.Owner);
            var cardArea = Card.Area;

            if (cardArea == Area.Hand)
                player.RemoveFromHand(Card);

            if (cardArea == Area.Field)
                player.RemoveFromField(Card);

            if (cardArea == Area.Deck)
                player.RemoveFromDeck(Card);

            if (cardArea == Area.Destroyed)
                player.RemoveFromDestroyed(Card);

            player.AddToEliminated(Card);

            if (CreateUIEvent && Area.PlayArea.HasFlag(cardArea))
                MainController.AddEvent(new DestroyCardUIEvent(Card));
        }
    }

    public class EmptyDestroyPileEvent : BaseCardEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Emptying Destroyed pile for {Player} Player" ;

        private PlayerType Player { get; set; }

        public EmptyDestroyPileEvent(PlayerType player)
        {
            Player = player;
        }

        public override void Process()
        {
            foreach (var card in MainController.GetPlayer(Player).GetDestroyed())
            {
                MainController.AddEvent(new EliminateCardEvent(card, false));
            }
        }
    }
}
