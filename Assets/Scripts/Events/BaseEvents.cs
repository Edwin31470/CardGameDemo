using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Cards;

namespace Assets.Scripts.Events
{
    public abstract class BaseEvent
    {
        public virtual string EventTitle => null;
        public virtual float Delay => 0;
    }

    public abstract class BaseUIEvent : BaseEvent
    {
        public abstract void Process(UIManager uIManager);
    }

    // Gameplay events are events that go in to the normal queue
    public abstract class BaseGameplayEvent : BaseEvent
    {
        public virtual IEnumerable<BaseEvent> Process()
        {
            throw new MethodAccessException($"{nameof(BaseGameplayEvent.Process)} should not be being called. Called by {GetType()}");
        }
    }

    // A board event is something that affects the board state (card stats, card ownership, player stats) - most things
    public abstract class BaseBoardEvent : BaseGameplayEvent
    {
        public virtual IEnumerable<BaseEvent> Process(BoardState board)
        {
            throw new MethodAccessException($"{nameof(BaseBoardEvent.Process)} should not be being called. Called by {GetType()}");
        }
    }

    // Like a board event but requires UI interaction
    public abstract class BaseUIInteractionEvent : BaseGameplayEvent
    {
        public virtual IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board)
        {
            throw new MethodAccessException($"{nameof(BaseUIInteractionEvent.Process)} should not be being called. Called by {GetType()}");
        }
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
