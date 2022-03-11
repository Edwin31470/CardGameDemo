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
        public BaseCard Owner { get; set; }
        public bool TriggerOnce { get; set; }

        protected BaseTriggerEvent(BaseCard owner, bool triggerOnce = false)
        {
            Owner = owner;
            TriggerOnce = triggerOnce;
        }

        public abstract bool Conditions(BaseEvent triggeringEvent);
        public abstract IEnumerable<BaseEvent> Process(BaseEvent triggeringEvent);

        public bool IsValid()
        {
            return Owner.Area == Area.Field;
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
            return triggeringEvent is DestroyCardEvent destroyCardEvent && destroyCardEvent.Card == Owner;
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
            if (triggeringEvent is EnterFieldEvent enterFieldEvent && enterFieldEvent.Card == Owner)
                return false;

            return FuncConditions.Invoke(triggeringEvent);
        }

        public override IEnumerable<BaseEvent> Process(BaseEvent triggeringEvent)
        {
            return Func.Invoke(triggeringEvent);
        }
    }
}
