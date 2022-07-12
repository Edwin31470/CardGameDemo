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

            player.RemoveFromField(Card);
            player.AddToHand(Card);

            yield return new UpdateHandUIEvent(player.PlayerType, player.Hand);
        }
    }

    public class EnterFieldEvent<T> : BaseAreaEvent<T>, IEnterFieldEvent where T : FieldCard
    {
        public FieldSlot Slot { get; set; }

        public EnterFieldEvent(T card, FieldSlot slot) : base(card)
        {
            Slot = slot;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            // Work out who owns the card
            var player = board.GetSourceOwner(Slot); // Field cards are always owned by the player whose side they are played on

            // Add card to slot
            if (player.IsInHand(Source))
                player.RemoveFromHand(Source);

            Slot.AddCard(Source);

            // Get the card's events
            foreach (var baseEvent in Source.GetEvents(board).ToList())
            {
                yield return baseEvent;
            }
        }
    }

    public class PlayActionCardEvent : BaseAreaEvent<ActionCard>
    {
        public PlayActionCardEvent(ActionCard source) : base(source)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            // Destroy
            yield return new DestroyCardEvent<ActionCard>(Source);

            // Get the card's events
            foreach (var baseEvent in Source.GetEvents(board).ToList()) {
                yield return baseEvent;
            }
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
                player.RemoveFromField(fieldCard);
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
                player.RemoveFromField(fieldCard);
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
}
