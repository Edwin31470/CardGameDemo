using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Cards;
using Assets.Scripts.Bases;
using Assets.Scripts.Events.Interfaces;

namespace Assets.Scripts.Events
{
    public abstract class BaseEvent
    {
        public virtual string EventTitle => null;
        public virtual float Delay => 0;
    }

    // An event that causes something to change in the UI (Creating a card, moving a card etc.)
    public abstract class BaseUIEvent : BaseEvent
    {
        public abstract void Process(UIManager uIManager);
    }

    // An event that has a logical source (A card, item, player etc.)
    // Should not be directly inherited from
    public abstract class BaseSourceEvent<T> : BaseEvent, ISourceEvent where T : BaseSource
    {
        public T Source { get; set; }
        public BaseSource BaseSource => Source;

        public bool IsPrevented { get; set; }

        protected BaseSourceEvent(T source)
        {
            Source = source;
        }

        public bool IsValid(BoardState board)
        {
            // Card events are only valid when their source is on the field
            if (Source is BaseCard card)
                return board.GetSourceOwner(Source).IsOnField(card);

            return true;
        }
    }

    // Events that happen once (i.e. not Passive or Trigger events) - most events
    public abstract class BaseOnceEvent<T> : BaseSourceEvent<T>, IOnceEvent where T : BaseSource
    {
        public BaseOnceEvent(T source) : base(source)
        {
        }
    }

    // Processes with a board and returns events
    public abstract class BaseGameplayEvent<T> : BaseOnceEvent<T>, IGameplayEvent where T : BaseSource
    {
        protected BaseGameplayEvent(T source) : base(source)
        {
        }

        public abstract IEnumerable<BaseEvent> Process(BoardState board);
    }

    // Like a gameplay event but requires UI interaction and thus the UIManager
    public abstract class BaseUIInteractionEvent<T> : BaseOnceEvent<T>, IUIInteractionEvent where T: BaseSource
    {
        protected BaseUIInteractionEvent(T source) : base(source)
        {
        }

        public abstract IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board);
    }

    // Processes with the board and an event and potentially modifies the event before that event is processed
    public abstract class BaseInteruptEvent<T> : BaseSourceEvent<T>, IInteruptEvent where T : BaseSource
    {
        public bool TriggerOnce { get; set; }

        protected BaseInteruptEvent(T source, bool triggerOnce) : base(source)
        {
            TriggerOnce = triggerOnce;
        }

        public abstract bool Process(BoardState boardState, IInteruptableEvent baseEvent);
    }

    // Processes with a board before any event is processed
    public abstract class BasePassiveEvent<T> : BaseSourceEvent<T>, IPassiveEvent where T : BaseSource
    {
        protected BasePassiveEvent(T source) : base(source)
        {
        }

        public abstract void Process(BoardState board);
    }

    public abstract class BasePhaseEvent : BaseEvent
    {
        public override float Delay => 1f;
        public abstract void Process(MainController controller, BoardState boardState);
    }

    public class MessageEvent : BaseUIEvent
    {
        public override string EventTitle => Message;
        public override float Delay => Time;

        private string Message { get; set; }
        private float Time { get; set; }

        public MessageEvent(string message, float time = 2)
        {
            Message = message;
            Time = time;
        }

        public override void Process(UIManager uIManager)
        {
        }
    }
}
