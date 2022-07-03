using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Assets.Scripts.Bases;
using System;
using Assets.Scripts.Events.Interfaces;

namespace Assets.Scripts.Events
{
    // Any event that may move cards from or to areas (hand, field, deck, destroyed, eliminated)
    // Usually returns a UI event
    public abstract class BaseAreaEvent<T> : BaseSourceEvent<T> where T : BaseSource
    {
        protected BaseAreaEvent(T source) : base(source)
        {
        }
    }

    public class DrawCardEvent<T> : BaseAreaEvent<T> where T : BaseSource
    {
        private PlayerType PlayerType { get; set; }

        public DrawCardEvent(T source, PlayerType playerType) : base(source)
        {
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);

            if (player.Deck.Count == 0)
            {
                yield return new GameEndEvent($"{player.PlayerType} Player has run out of cards!");
                yield break;
            };

            var card = player.TakeFromDeck();
            player.AddToHand(card);

            yield return new DrawCardUIEvent(PlayerType, card, player.Hand);
        }
    }

    public class ReturnToDeckEvent<T> : BaseAreaEvent<T> where T : BaseSource
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckEvent(T source, BaseCard card) : base(source)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetSourceOwner(Card);

            player.RemoveFromHand(Card);
            player.AddToDeck(Card);

            yield return new ReturnToDeckUIEvent(player.PlayerType, Card);
        }
    }

    public class ReturnFromFieldToHandEvent<T> : BaseAreaEvent<T> where T : BaseSource
    {
        private FieldCard Card { get; set; }

        public ReturnFromFieldToHandEvent(T source, FieldCard card) : base(source)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetSourceOwner(Card);

            var slotEvents = player.RemoveFromField(Card);
            foreach (var slotEvent in slotEvents) {
                yield return slotEvent;
            }
            player.AddToHand(Card);

            yield return new UpdateHandUIEvent(player.PlayerType, player.Hand);
        }
    }

    // Enter field for creature and permanent cards
    public class EnterFieldEvent<T> : BaseAreaEvent<T> where T : BaseCard
    {
        private FieldSlot Slot { get; set; } // null for action card

        public EnterFieldEvent(T card, FieldSlot slot) : base(card)
        {
            Slot = slot;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            if (Source is FieldCard fieldCard)
            {
                // Work out who owns the card
                var player = board.GetSourceOwner(Slot); // Field cards are always owned by the player whose side they are played on

                // Add card to slot
                if (player.IsInHand(fieldCard))
                    player.RemoveFromHand(fieldCard);

                var slotEvents = Slot.Add(fieldCard);
                foreach (var slotEvent in slotEvents)
                {
                    yield return slotEvent;
                }

                // Get the card's events
                foreach (var baseEvent in Source.GetEvents(board).ToList())
                {
                    yield return baseEvent;
                }

                yield break;
            }
            else if (Source is ActionCard actionCard)
            {
                // Destroy
                yield return new DestroyCardEvent<ActionCard>(actionCard);

                // Get the card's events
                foreach (var baseEvent in Source.GetEvents(board).ToList())
                {
                    yield return baseEvent;
                }

                yield break;
            }

            throw new ArgumentOutOfRangeException(nameof(Source), $"Card type must be {typeof(FieldCard)} or {typeof(ActionCard)}");
        }
    }

    public class DestroyCardEvent<T> : BaseAreaEvent<T>, IDestroyCardEvent where T : BaseCard
    {
        public DestroyCardEvent(T card) : base(card)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetSourceOwner(Source);

            // stop duplicate destroys - a better way to do this?
            if (player.IsInDestroyed(Source) || player.IsInEliminated(Source))
                yield break;

            // From hand
            if (player.IsInHand(Source)) {
                player.RemoveFromHand(Source);
                yield return new DestroyCardUIEvent(player.PlayerType, Source);
            }

            // From field
            if (Source is FieldCard fieldCard && player.IsOnField(Source)) {
                var slotEvents = player.RemoveFromField(fieldCard);
                foreach (var slotEvent in slotEvents) {
                    yield return slotEvent;
                }
                yield return new DestroyCardUIEvent(player.PlayerType, Source);
            }

            // From deck
            if (player.IsInDeck(Source)) {
                player.RemoveFromDeck(Source);
            }

            // Add to pile
            if (!Source.IsSummoned) {
                player.AddToDestroyed(Source);
            }
            else
            {
                player.AddToEliminated(Source);
            }
        }
    }

    public class DestroyByDamageEvent : BaseAreaEvent<CreatureCard>
    {
        public DestroyByDamageEvent(CreatureCard card) : base(card)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            // Check card hasn't now got more defence
            if (Source.Defence > 0)
                yield break;

            yield return new DestroyCardEvent<CreatureCard>(Source);
        }
    }

    public class EliminateCardEvent<T> : BaseAreaEvent<T> where T : BaseCard
    {
        private bool CreateUIEvent { get; set; }

        public EliminateCardEvent(T card, bool createUIEvent = true) : base(card)
        {
            CreateUIEvent = createUIEvent;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetSourceOwner(Source);

            if (player.IsInHand(Source))
                player.RemoveFromHand(Source);

            if (Source is FieldCard fieldCard && player.IsOnField(Source)) {
                var slotEvents = player.RemoveFromField(fieldCard);
                foreach (var slotEvent in slotEvents) {
                    yield return slotEvent;
                }
            }

            if (player.IsInDeck(Source))
                player.RemoveFromDeck(Source);

            if (player.IsInDestroyed(Source))
                player.RemoveFromDestroyed(Source);

            player.AddToEliminated(Source);

            if (CreateUIEvent && player.IsInHand(Source) && player.IsOnField(Source))
                yield return new DestroyCardUIEvent(player.PlayerType, Source);
        }
    }

    public class ManaSacrificeEvent : BaseAreaEvent<Player>
    {
        private BaseCard Card { get; set; }

        public ManaSacrificeEvent(Player player, BaseCard card) : base(player)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.RemoveFromHand(Card);
            Source.AddToEliminated(Card);

            yield return new SacrificeCardUIEvent(Card);
        }
    }

    public class EmptyDestroyPileEvent : BaseAreaEvent<Player>
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Emptying Destroyed pile for {Source.PlayerType} Player" ;

        public EmptyDestroyPileEvent(Player player) : base(player)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            foreach (var card in Source.Destroyed)
            {
                yield return new EliminateCardEvent<BaseCard>(card, false);
            }
        }
    }

    public class GainControlEvent<T> : BaseAreaEvent<T> where T : FieldCard
    {
        public GainControlEvent(T card) : base(card)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var originalPlayer = board.GetSourceOwner(Source);
            var newPlayer = board.GetPlayer(originalPlayer.PlayerType.Opposite());

            var leaveSlotEvents = originalPlayer.RemoveFromField(Source);
            foreach (var slotEvent in leaveSlotEvents) {
                yield return slotEvent;
            }

            var slot = newPlayer.GetRandomEmptySlot();
            if (slot == null) {
                yield return new DestroyCardUIEvent(originalPlayer.PlayerType, Source);
            }
            else
            {
                var enterSlotEvents = slot.Add(Source);
                foreach (var slotEvent in enterSlotEvents) {
                    yield return slotEvent;
                }
                yield return new MoveCardToFieldUIEvent(Source, slot);
            }
        }
    }

    public class SummonCardEvent : BaseAreaEvent<Player>
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Summoning {CardInfo.CardData.Name}";

        private CardInfo CardInfo { get; set; }
        private PlayerType PlayerType { get; set; }

        public SummonCardEvent(Player player, CardInfo cardInfo, PlayerType playerType) : base(player)
        {
            cardInfo.IsSummoned = true;
            CardInfo = cardInfo;
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);
            var summonedCard = BaseCard.Create(CardInfo);

            if (summonedCard is ActionCard actionCard)
            {
                yield return new EnterFieldEvent<ActionCard>(actionCard, null);
            }
            else if (summonedCard is FieldCard fieldCard)
            {
                var slot = player.GetRandomEmptySlot();
                if (slot == null)
                {
                    yield return new MessageEvent("Unable to summon card: no free slots", 1);
                    yield break;
                }

                yield return new CreateCardInSlotUIEvent(PlayerType, summonedCard, slot);
                yield return new EnterFieldEvent<FieldCard>(fieldCard, slot);
            }
        }
    }

}
