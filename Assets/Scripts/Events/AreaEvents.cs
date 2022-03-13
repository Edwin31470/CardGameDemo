using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);

            var card = player.TakeFromDeck();
            player.AddToHand(card);

            yield return new DrawCardUIEvent(PlayerType, card, player.Hand);
        }
    }

    public class ReturnToDeckEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            player.RemoveFromHand(Card);
            player.AddToDeck(Card);

            yield return new ReturnToDeckUIEvent(player.PlayerType, Card);
        }
    }

    public class ReturnFromFieldToHandEvent : BaseAreaEvent
    {
        private FieldCard Card { get; set; }

        public ReturnFromFieldToHandEvent(FieldCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            var slotEvents = player.RemoveFromField(Card);
            foreach (var slotEvent in slotEvents) {
                yield return slotEvent;
            }
            player.AddToHand(Card);

            yield return new UpdateHandUIEvent(player.PlayerType, player.Hand);
        }
    }

    public class EnterFieldEvent : BaseAreaEvent
    {
        public BaseCard Card { get; set; }
        private FieldSlot Slot { get; set; } // null for action card

        public EnterFieldEvent(BaseCard card, FieldSlot slot)
        {
            Card = card;
            Slot = slot;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            if (player.IsInHand(Card))
                player.RemoveFromHand(Card);

            if (Slot == null)
            {
                yield return new DestroyCardEvent(Card);
            }
            else
            {
                var slotEvents = Slot.Add((FieldCard)Card);
                foreach (var slotEvent in slotEvents)
                {
                    yield return slotEvent;
                }
            }

            foreach (var baseEvent in Card.EffectEvents.Invoke())
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            // stop duplicate destroys - a better way to do this?
            if (player.IsInDestroyed(Card) || player.IsInEliminated(Card))
                yield break;

            if (player.IsInHand(Card)) {
                player.RemoveFromHand(Card);
                yield return new DestroyCardUIEvent(player.PlayerType, Card);
            }

            if (Card is FieldCard fieldCard && player.IsOnField(Card)) {
                var slotEvents = player.RemoveFromField(fieldCard);
                foreach (var slotEvent in slotEvents) {
                    yield return slotEvent;
                }
                yield return new DestroyCardUIEvent(player.PlayerType, Card);
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            if (player.IsInHand(Card))
                player.RemoveFromHand(Card);

            if (Card is FieldCard fieldCard && player.IsOnField(Card)) {
                var slotEvents = player.RemoveFromField(fieldCard);
                foreach (var slotEvent in slotEvents) {
                    yield return slotEvent;
                }
            }

            if (player.IsInDeck(Card))
                player.RemoveFromDeck(Card);

            if (player.IsInDestroyed(Card))
                player.RemoveFromDestroyed(Card);

            player.AddToEliminated(Card);

            if (CreateUIEvent && player.IsInHand(Card) && player.IsOnField(Card))
                yield return new DestroyCardUIEvent(player.PlayerType, Card);
        }
    }

    public class ManaSacrificeEvent : BaseAreaEvent
    {
        private BaseCard Card { get; set; }

        public ManaSacrificeEvent(BaseCard card)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetCardOwner(Card);

            player.RemoveFromHand(Card);
            player.AddToEliminated(Card);

            yield return new SacrificeCardUIEvent(Card);
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            foreach (var card in board.GetPlayer(Player).Destroyed)
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

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var originalPlayer = board.GetCardOwner(Card);
            var newPlayer = board.GetPlayer(originalPlayer.PlayerType.GetOpposite());

            var leaveSlotEvents = originalPlayer.RemoveFromField(Card);
            foreach (var slotEvent in leaveSlotEvents) {
                yield return slotEvent;
            }

            var slot = newPlayer.GetRandomEmptySlot();
            if (slot == null) {
                yield return new DestroyCardUIEvent(originalPlayer.PlayerType, Card);
            }
            else
            {
                var enterSlotEvents = slot.Add(Card);
                foreach (var slotEvent in enterSlotEvents) {
                    yield return slotEvent;
                }
                yield return new MoveCardToFieldUIEvent(Card, slot);
            }
        }
    }

    public class SummonCardEvent : BaseAreaEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Summoning {CardInfo.CardData.Name}";

        private CardInfo CardInfo { get; set; }
        private PlayerType PlayerType { get; set; }

        public SummonCardEvent(CardInfo cardInfo, PlayerType playerType)
        {
            cardInfo.IsSummoned = true;
            CardInfo = cardInfo;
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);
            var summonedCard = BaseCard.Create(CardInfo);

            if (summonedCard is FieldCard fieldCard)
            {
                var slot = player.GetRandomEmptySlot();
                if (slot == null)
                {
                    yield return new MessageEvent("Unable to summon card: no free slots", 1);
                    yield break;

                }

                yield return new CreateCardInSlotUIEvent(PlayerType, fieldCard, slot);
                yield return new EnterFieldEvent(fieldCard, slot);
            }
            else
            {
                yield return new EnterFieldEvent(summonedCard, null);
            }
        }
    }

}
