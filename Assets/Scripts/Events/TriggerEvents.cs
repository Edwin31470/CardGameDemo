using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public abstract class BaseTriggerEvent<T> : BaseSourceEvent<T>, ITriggerEvent where T : BaseSource
    {
        public bool TriggerOnce { get; set; }

        protected BaseTriggerEvent(T source, bool triggerOnce = false) : base(source)
        {
            TriggerOnce = triggerOnce;
        }

        public abstract bool Conditions(BoardState boardState, BaseEvent triggeringEvent);
        public abstract IEnumerable<BaseEvent> Process(BoardState boardState, BaseEvent triggeringEvent);
    }


    public class OnDestroyedEvent<T> : BaseTriggerEvent<T> where T : BaseSource
    {
        // Function to trigger when the card is destroyed
        private Func<T, BoardState, IEnumerable<BaseEvent>> Func { get; set; }

        public OnDestroyedEvent(T owner, Func<T, BoardState, IEnumerable<BaseEvent>> func) : base(owner, true)
        {
            Func = func;
        }

        public override bool Conditions(BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is IDestroyCardEvent destroyCardEvent && destroyCardEvent.BaseSource == Source;
        }

        public override IEnumerable<BaseEvent> Process(BoardState boardState, BaseEvent triggeringEvent)
        {
            return Func.Invoke(Source, boardState);
        }
    }

    public class CustomTriggerEvent<T> : BaseTriggerEvent<T> where T : BaseSource
    {
        private Func<T, BoardState, BaseEvent, bool> FuncConditions { get; } 
        private Func<T, BoardState, BaseEvent, IEnumerable<BaseEvent>> Func { get; }

        public CustomTriggerEvent(T owner, Func<T, BoardState, BaseEvent, bool> conditions, Func<T, BoardState, BaseEvent, IEnumerable<BaseEvent>> func, bool triggerOnce = false) : base(owner, triggerOnce)
        {
            FuncConditions = conditions;
            Func = func;
        }

        public override bool Conditions(BoardState boardState, BaseEvent triggeringEvent)
        {
            // Prevents cards triggering on themselves
            if (triggeringEvent is IEnterFieldEvent enterFieldEvent && enterFieldEvent.BaseSource == Source)
                return false;

            return FuncConditions.Invoke(Source, boardState, triggeringEvent);
        }

        public override IEnumerable<BaseEvent> Process(BoardState boardState, BaseEvent triggeringEvent)
        {
            return Func.Invoke(Source, boardState, triggeringEvent);
        }
    }

    public class CustomOnGameStartEvent<T> : CustomTriggerEvent<T> where T : BaseSource
    {
        public CustomOnGameStartEvent(T owner, Func<T, BoardState, BaseEvent, IEnumerable<BaseEvent>> onRoundStart) : base(owner, IsGameStart, onRoundStart)
        {
        }

        private static bool IsGameStart(T source, BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is NewPhaseEvent newPhaseEvent && newPhaseEvent.Phase == Phase.GameStart;
        }
    }

    public class CustomOnRoundStartEvent<T> : CustomTriggerEvent<T> where T : BaseSource
    {
        public CustomOnRoundStartEvent(T owner, Func<T, BoardState, BaseEvent, IEnumerable<BaseEvent>> onRoundStart) : base(owner, IsRoundStart, onRoundStart)
        {
        }

        private static bool IsRoundStart(T source, BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is NewPhaseEvent newPhaseEvent && newPhaseEvent.Phase == Phase.Play;
        }
    }
}
