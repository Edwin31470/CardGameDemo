using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    // An event that can be interupted by an interupt event
    public interface IInteruptableEvent
    {
        bool IsPrevented { get; set; }
    }
}
