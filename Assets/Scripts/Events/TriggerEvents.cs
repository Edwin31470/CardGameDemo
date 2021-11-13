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

        protected BaseTriggerEvent(BaseCard owner)
        {
            Owner = owner;
        }

        public abstract bool Process(BaseEvent triggeringEvent);

        public bool IsValid()
        {
            return Owner.Area == Area.Field;
        }
    }

    public abstract class BaseTriggerOnceEvent : BaseTriggerEvent
    {
        protected BaseTriggerOnceEvent(BaseCard owner) : base(owner)
        {
        }
    }

    public class OnDestroyedEvent : BaseTriggerOnceEvent
    {
        private Action Action { get; set; }

        public OnDestroyedEvent(BaseCard owner, Action action) : base(owner)
        {
            Action = action;
        }

        public override bool Process(BaseEvent triggeringEvent)
        {
            if (triggeringEvent is DestroyCardEvent destroyCardEvent && destroyCardEvent.Card == Owner)
            {
                Action.Invoke();
                return true;
            }

            return false;
        }
    }

    public class CustomTriggerEvent : BaseTriggerEvent
    {
        private Func<BaseEvent, bool> Func { get; set; }

        public CustomTriggerEvent(BaseCard owner, Func<BaseEvent, bool> func) : base(owner)
        {
            Func = func;
        }

        public override bool Process(BaseEvent triggeringEvent)
        {
            // Prevents cards triggering on themselves
            if (triggeringEvent is EnterFieldEvent enterFieldEvent && enterFieldEvent.Card == Owner)
                return false;

            return Func.Invoke(triggeringEvent);
        }
    }

    public class CustomTriggerOnceEvent : BaseTriggerOnceEvent
    {
        private Func<BaseEvent, bool> Func { get; set; }

        public CustomTriggerOnceEvent(BaseCard owner, Func<BaseEvent, bool> func) : base(owner)
        {
            Func = func;
        }

        public override bool Process(BaseEvent triggeringEvent)
        {
            // Prevents cards triggering on themselves
            if (triggeringEvent is EnterFieldEvent enterFieldEvent && enterFieldEvent.Card == Owner)
                return false;

            return Func.Invoke(triggeringEvent);
        }
    }
}
