using Assets.Scripts.UI;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    public abstract class BaseEvent
    {
        public virtual string EventTitle => null;
        public virtual float Delay => 0;

        /// <summary>
        /// Actions a delegate
        /// </summary>
        /// <returns>Enumerable of events to queue</returns>
        public virtual IEnumerable<BaseEvent> Process()
        {
            throw new MethodAccessException($"BaseEvent.Process() should not be being called. Called by {GetType()}");
        }
    }

    public abstract class BaseUIEvent : BaseEvent
    {
        public abstract void Process(UIManager uIManager);
    }

    // Gameplay events are events that go in to the normal queue
    public abstract class BaseGameplayEvent : BaseEvent
    {
    }


    public abstract class BaseUIInteractionEvent : BaseGameplayEvent
    {
        public abstract void Process(UIManager uIManager);
    }

    public abstract class BasePhaseEvent : BaseEvent
    {
        public override float Delay => 1f;
        public abstract void Process(MainController controller);
    }


    public class MessageEvent : BaseGameplayEvent
    {
        public override string EventTitle => Message;
        public override float Delay => Time;

        private string Message { get; set; }
        private float Time { get; set; }

        public MessageEvent(string message, float time)
        {
            Message = message;
            Time = time;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            yield break;
        }
    }
}
