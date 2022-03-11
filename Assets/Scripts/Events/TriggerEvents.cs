using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public abstract class BaseTriggerEvent : BaseEvent
    {
        public BaseCard Source { get; set; }
        public bool TriggerOnce { get; set; }

        protected BaseTriggerEvent(BaseCard source, bool triggerOnce = false)
        {
            Source = source;
            TriggerOnce = triggerOnce;
        }

        public abstract bool Conditions(BaseEvent triggeringEvent);
        public abstract IEnumerable<BaseEvent> Process(BaseEvent triggeringEvent);

        // Trigger events are only valid when their source is on the field
        public bool IsValid()
        {
            return Source.Owner.IsOnField(Source);
        }
    }


    public class OnDestroyedEvent : BaseTriggerEvent
    {
        private Func<IEnumerable<BaseEvent>> Func { get; set; }

        public OnDestroyedEvent(BaseCard owner, Func<IEnumerable<BaseEvent>> func) : base(owner, true)
        {
            Func = func;
        }


        public override bool Conditions(BaseEvent triggeringEvent)
        {
            return triggeringEvent is DestroyCardEvent destroyCardEvent && destroyCardEvent.Card == Source;
        }

        public override IEnumerable<BaseEvent> Process(BaseEvent triggeringEvent)
        {
            return Func.Invoke();
        }
    }

    public class CustomTriggerEvent : BaseTriggerEvent
    {
        private Func<BaseEvent, bool> FuncConditions { get; } 
        private Func<BaseEvent, IEnumerable<BaseEvent>> Func { get; }

        public CustomTriggerEvent(BaseCard owner, Func<BaseEvent, bool> conditions, Func<BaseEvent, IEnumerable<BaseEvent>> func, bool triggerOnce = false) : base(owner, triggerOnce)
        {
            FuncConditions = conditions;
            Func = func;
        }

        public override bool Conditions(BaseEvent triggeringEvent)
        {
            // Prevents cards triggering on themselves
            if (triggeringEvent is EnterFieldEvent enterFieldEvent && enterFieldEvent.Card == Source)
                return false;

            return FuncConditions.Invoke(triggeringEvent);
        }

        public override IEnumerable<BaseEvent> Process(BaseEvent triggeringEvent)
        {
            return Func.Invoke(triggeringEvent);
        }
    }
}
