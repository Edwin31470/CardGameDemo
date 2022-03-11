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

    public class ReturnFromFieldToHandEvent : BaseAreaEvent
    {
        private FieldCard Card { get; set; }

        public ReturnFromFieldToHandEvent(FieldCard card)
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
        private int Index { get; set; }

        public EnterFieldEvent(BaseCard card, int index)
        {
            Card = card;
            Index = index;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = Card.Owner;

            if (player.IsInHand(Card))
                player.RemoveFromHand(Card);

            if (Card is FieldCard fieldCard)
            {
                player.AddToField(fieldCard, Index);
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
            var player = Card.Owner;

            // stop duplicate destroys - a better way to do this?
            if (player.IsInDestroyed(Card) || player.IsInEliminated(Card))
                yield break;

            if (player.IsInHand(Card)) {
                player.RemoveFromHand(Card);
                yield return new DestroyCardUIEvent(Card);
            }

            if (Card is FieldCard fieldCard && player.IsOnField(Card)) {
                player.RemoveFromField(fieldCard);
                yield return new DestroyCardUIEvent(Card);
            }

            if (player.IsInDeck(Card)) {
                player.RemoveFromDeck(Card);

            }

            if (!Card.IsSummoned) {
                player.AddToDestroyed(Card);
            }
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

            if (player.IsInHand(Card))
                player.RemoveFromHand(Card);

            if (Card is FieldCard fieldCard && player.IsOnField(Card))
                player.RemoveFromField(fieldCard);

            if (player.IsInDeck(Card))
                player.RemoveFromDeck(Card);

            if (player.IsInDestroyed(Card))
                player.RemoveFromDestroyed(Card);

            player.AddToEliminated(Card);

            if (CreateUIEvent && player.IsInHand(Card) && player.IsOnField(Card))
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
        private FieldCard Card { get; set; }

        public GainControlEvent(FieldCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var originalPlayer = Card.Owner;
            var newPlayer = getPlayer(Card.Owner.PlayerType.GetOpposite());

            originalPlayer.RemoveFromField(Card);
            Card.Owner = newPlayer;

            var index = newPlayer.GetRandomEmptySlot();
            if (index == -1) {
                yield return new DestroyCardUIEvent(Card);
            }
            else
            {
                newPlayer.AddToField(Card, index);
                yield return new MoveCardToFieldUIEvent(Card, index);
            }
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
            var player = getPlayer(PlayerType);
            var summonedCard = BaseCard.Create(getPlayer(PlayerType), CardInfo);

            if (summonedCard is FieldCard fieldCard)
            {
                var index = player.GetRandomEmptySlot();
                if (index == -1)
                {
                    yield return new MessageEvent("Unable to summon card: no free slots", 1);
                    yield break;

                }

                yield return new CreateCardInSlotUIEvent(fieldCard, index);
                yield return new EnterFieldEvent(fieldCard, index);
            }
            else
            {
                yield return new EnterFieldEvent(summonedCard, -1);
            }
        }
    }

}
