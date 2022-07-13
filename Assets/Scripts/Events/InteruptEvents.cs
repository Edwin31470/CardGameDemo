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


    public class CustomInteruptEvent<T> : BaseInteruptEvent<T> where T : BaseSource
    {
        private Func<T, BoardState, IInteruptableEvent, bool> Interupt { get; set; }

        public CustomInteruptEvent(T source, Func<T, BoardState, IInteruptableEvent, bool> interupt, bool triggerOnce = false) : base(source, triggerOnce)
        {
            Interupt = interupt;
        }

        public override bool Process(BoardState boardState, IInteruptableEvent baseEvent)
        {
            return Interupt.Invoke(Source, boardState, baseEvent);
        }
    }
}
