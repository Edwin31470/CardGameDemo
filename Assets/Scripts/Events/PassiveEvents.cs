using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public abstract class BasePassiveEvent : BaseEvent
    {
        public BaseCard Source { get; set; }

        protected BasePassiveEvent(BaseCard source)
        {
            Source = source;
        }

        public abstract void Process(BoardState board);

        // Passive events are only valid when their source is on the field
        public bool IsValid(BoardState board)
        {
            return board.GetCardOwner(Source).IsOnField(Source);
        }
    }

    // For events without targets
    public class CustomPassiveEvent : BasePassiveEvent
    {
        private Action Action { get; set; } // Owner - Targets

        public CustomPassiveEvent(BaseCard owner, Action action) : base(owner)
        {
            Action = action;
        }

        public override void Process(BoardState board)
        {
            Action.Invoke();
        }
    }

    // For events with creature targets
    public class CustomPassiveAllCreaturesEvent<T> : BasePassiveEvent where T : BaseCard
    {
        private TargetConditions TargetConditions { get; set; } // Filter out targets not matching these conditions
        private Action<T, BoardState, IEnumerable<CreatureCard>> Action { get; set; } // Targets

        public CustomPassiveAllCreaturesEvent(T source, TargetConditions targetConditions, Action<T, BoardState, IEnumerable<CreatureCard>> action)
            : base(source)
        {
            targetConditions.CardType = CardType.Creature;
            TargetConditions = targetConditions;
            Action = action;
        }

        public override void Process(BoardState board)
        {
            var cards = board.GetMatchingCards(TargetConditions).Cast<CreatureCard>();
            Action.Invoke((T)Source, board, cards);
        }
    }
}
