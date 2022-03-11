using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using Assets.Scripts.Extensions;

namespace Assets.Scripts.Events
{
    // Any event that may move cards from or to areas (hand, field, deck, destroyed, eliminated)
    // Usually returns a UI event
    public abstract class BaseAreaEvent : BaseBoardEvent
    {

    }

    public class DrawCardEvent : BaseAreaEvent
    {
        private PlayerType PlayerType { get; set; }

        public DrawCardEvent(PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = getPlayer(PlayerType);

            var card = player.TakeFromDeck();
            player.AddToHand(card);

            yield return new DrawCardUIEvent(card);
        }
    }

    public class ReturnToDeckEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = Card.Owner;

            player.RemoveFromHand(Card);
            player.AddToDeck(Card);

            yield return new ReturnToDeckUIEvent(Card);
        }
    }

    public class ReturnToHandEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToHandEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = Card.Owner;

            player.RemoveFromField(Card);
            player.AddToHand(Card);

            yield return new AddToHandUIEvent(Card);
        }
    }

    public class EnterFieldEvent : BaseAreaEvent
    {
        public BaseCard Card { get; set; }

        public EnterFieldEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = Card.Owner;

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

    public class DestroyCardEvent : BaseAreaEvent
    {
        public BaseCard Card { get; set; }

        public DestroyCardEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            // stop duplicate destroys - a better way to do this?
            if (Card.Area != Area.None && Area.Pile.HasFlag(Card.Area))
                yield break;

            var player = Card.Owner;
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

    public class DestroyCreatureByDamageEvent : BaseAreaEvent
    {
        public CreatureCard Card { get; set; }

        public DestroyCreatureByDamageEvent(CreatureCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            // Check card hasn't now got more defence
            if (Card.Defence > 0)
                yield break;

            yield return new DestroyCardEvent(Card);
        }
    }

    public class EliminateCardEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }
        private bool CreateUIEvent { get; set; }

        public EliminateCardEvent(BaseCard card, bool createUIEvent = true)
        {
            Card = card;
            CreateUIEvent = createUIEvent;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = Card.Owner;
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

    public class EmptyDestroyPileEvent : BaseAreaEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Emptying Destroyed pile for {Player} Player" ;

        private PlayerType Player { get; set; }

        public EmptyDestroyPileEvent(PlayerType player)
        {
            Player = player;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            foreach (var card in getPlayer(Player).GetDestroyed())
            {
                yield return new EliminateCardEvent(card, false);
            }
        }
    }

    public class GainControlEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }

        public GainControlEvent(BaseCard baseCard)
        {
            Card = baseCard;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var originalPlayer = Card.Owner;
            originalPlayer.RemoveFromField(Card);

            var newPlayer = getPlayer(Card.Owner.PlayerType.GetOpposite());
            Card.Owner = newPlayer;
            newPlayer.AddToField(Card);

            yield return new MoveCardToFieldUIEvent(Card);
        }
    }

    public class SummonCardEvent : BaseAreaEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Summoning {CardInfo.Name}";

        private CardInfo CardInfo { get; set; }
        private PlayerType PlayerType { get; set; }

        public SummonCardEvent(CardInfo cardInfo, PlayerType playerType)
        {
            cardInfo.IsSummoned = true;
            CardInfo = cardInfo;
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var summonedCard = BaseCard.Create(getPlayer(PlayerType), CardInfo);

            if (summonedCard.Type != CardType.Action)
                yield return new CreateCardInSlotUIEvent(summonedCard);

            yield return new EnterFieldEvent(summonedCard);
        }
    }

}
