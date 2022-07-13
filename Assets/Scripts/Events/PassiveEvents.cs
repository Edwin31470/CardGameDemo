using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    // For events without targets
    // Not used
    public class CustomPassiveEvent<T> : BasePassiveEvent<T>, IPassiveEvent where T : BaseSource
    {
        private Action Action { get; set; }

        public CustomPassiveEvent(T source, Action action) : base(source)
        {
            Action = action;
        }

        public override void Process(BoardState board)
        {
            Action.Invoke();
        }
    }

    // For events with creature targets
    public class CustomPassiveAllEvent<TSource, TTarget> : BasePassiveEvent<TSource>
        where TSource : BaseSource
        where TTarget : ITargetable
    {
        private TargetConditions TargetConditions { get; set; } // Filter out targets not matching these conditions
        private Action<TSource, BoardState, IEnumerable<TTarget>> Action { get; set; }

        public CustomPassiveAllEvent(TSource source, TargetConditions targetConditions, Action<TSource, BoardState, IEnumerable<TTarget>> action) : base(source)
        {
            TargetConditions = targetConditions;
            Action = action;
        }

        public override void Process(BoardState board)
        {
            var cards = board.GetMatchingTargets<TTarget>(TargetConditions);
            Action.Invoke(Source, board, cards);
        }
    }
}
