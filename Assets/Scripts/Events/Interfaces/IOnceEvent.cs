using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    // Once events go into the normal queue and are processed once - most events are IOnceEvents
    public interface IOnceEvent : ISourceEvent, ITriggeringEvent
    {
    }
}
