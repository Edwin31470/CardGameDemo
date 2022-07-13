using Assets.Scripts.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    public interface ISourceEvent : IInteruptableEvent, ITriggeringEvent
    {
        BaseSource BaseSource { get; }
        bool IsValid(BoardState board);
    }
}
