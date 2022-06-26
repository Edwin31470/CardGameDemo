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
    // Modifies an event before it goes off
    public abstract class BaseInteruptEvent<T> : BaseSourceEvent<T>, IInteruptEvent where T : BaseSource
    {
        public bool TriggerOnce { get; set; }

        protected BaseInteruptEvent(T source, bool triggerOnce) : base(source)
        {
            TriggerOnce = triggerOnce;
        }

        public abstract bool Process(BoardState boardState, BaseEvent baseEvent);
    }

    public class CustomInteruptEvent<T> : BaseInteruptEvent<T> where T : BaseSource
    {
        private Func<T, BoardState, BaseEvent, bool> Func { get; set; }

        public CustomInteruptEvent(T source, Func<T, BoardState, BaseEvent, bool> func, bool triggerOnce = false) : base(source, triggerOnce)
        {
            Func = func;
        }

        public override bool Process(BoardState boardState, BaseEvent baseEvent)
        {
            return Func.Invoke(Source, boardState, baseEvent);
        }
    }
}
