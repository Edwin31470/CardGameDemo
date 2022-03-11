using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    // Any event that may move cards from or to UI piles (hand, deck, discard, eliminated)
    public abstract class BasePileEvent : BaseGameplayEvent
    {
    }

    public class DrawCardEvent : BasePileEvent
    {
        private PlayerType PlayerType { get; set; }

        public DrawCardEvent(PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(PlayerType);

            var card = player.TakeFromDeck();
            player.AddToHand(card);

            yield return new DrawCardUIEvent(card);
        }
    }

    public class ReturnToDeckEvent : BasePileEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(Card.Owner);

            player.RemoveFromHand(Card);
            player.AddToDeck(Card);

            yield return new ReturnToDeckUIEvent(Card);
        }
    }

    public class ReturnToHandEvent : BasePileEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToHandEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(Card.Owner);

            player.RemoveFromField(Card);
            player.AddToHand(Card);

            yield return new AddToHandUIEvent(Card);
        }
    }


    public class EnterFieldEvent : BasePileEvent
    {
        public BaseCard Card { get; set; }

        public EnterFieldEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process()
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
                yield return new DestroyCardEvent(Card);
            }

            foreach (var baseEvent in Card.CardEvents)
            {
                yield return baseEvent;
            }
        }
    }

    public class DestroyCardEvent : BasePileEvent
    {
        public BaseCard Card { get; set; }

        public DestroyCardEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            // stop duplicate destroys - a better way to do this?
            if (Card.Area != Area.None && Area.Pile.HasFlag(Card.Area))
                yield break;

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
                yield return new DestroyCardUIEvent(Card);
        }
    }

    public class DestroyCreatureByDamageEvent : BasePileEvent
    {
        public CreatureCard Card { get; set; }

        public DestroyCreatureByDamageEvent(CreatureCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            // Check card hasn't now got more defence
            if (Card.Defence > 0)
                yield break;

            yield return new DestroyCardEvent(Card);
        }
    }

    public class EliminateCardEvent : BasePileEvent
    {
        private BaseCard Card { get; set; }
        private bool CreateUIEvent { get; set; }

        public EliminateCardEvent(BaseCard card, bool createUIEvent = true)
        {
            Card = card;
            CreateUIEvent = createUIEvent;
        }

        public override IEnumerable<BaseEvent> Process()
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
                yield return new DestroyCardUIEvent(Card);
        }
    }

    public class EmptyDestroyPileEvent : BasePileEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Emptying Destroyed pile for {Player} Player" ;

        private PlayerType Player { get; set; }

        public EmptyDestroyPileEvent(PlayerType player)
        {
            Player = player;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            foreach (var card in MainController.GetPlayer(Player).GetDestroyed())
            {
                yield return new EliminateCardEvent(card, false);
            }
        }
    }
}
