using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    // Like Triggers but modifies an event before it goes off
    public abstract class BaseInteruptEvent : BaseEvent
    {
        public BaseCard Owner { get; set; }

        protected BaseInteruptEvent(BaseCard owner)
        {
            Owner = owner;
        }

        public abstract bool Process(BaseGameplayEvent baseEvent);

        public bool IsValid()
        {
            return Owner.Area == Area.Field;
        }
    }

    public abstract class BaseInteruptOnceEvent : BaseInteruptEvent
    {
        protected BaseInteruptOnceEvent(BaseCard owner) : base(owner)
        {

        }
    }

    public class CustomInteruptEvent : BaseInteruptEvent
    {
        private Func<BaseGameplayEvent, bool> Func { get; set; }

        public CustomInteruptEvent(BaseCard owner, Func<BaseGameplayEvent, bool> func) : base(owner)
        {
            Func = func;
        }

        public override bool Process(BaseGameplayEvent baseEvent)
        {
            return Func.Invoke(baseEvent);
        }
    }

    public class CustomInteruptOnceEvent : BaseInteruptOnceEvent
    {
        private Func<BaseGameplayEvent, bool> Func { get; set; }

        public CustomInteruptOnceEvent(BaseCard owner, Func<BaseGameplayEvent, bool> func) : base(owner)
        {
            Func = func;
        }

        public override bool Process(BaseGameplayEvent triggeringEvent)
        {
            return Func.Invoke(triggeringEvent);
        }
    }
}
