using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    public interface ITriggerEvent : ISourceEvent
    {
        bool TriggerOnce { get; set; }

        bool Conditions(BoardState boardState, ITriggeringEvent triggeringEvent);
        IEnumerable<BaseEvent> Process(BoardState boardState, ITriggeringEvent triggeringEvent);
    }
}
