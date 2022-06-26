using Assets.Scripts.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    public interface IInteruptEvent : ISourceEvent
    {
        bool TriggerOnce { get; set; }

        bool Process(BoardState boardState, BaseEvent baseEvent);
    }
}
