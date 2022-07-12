﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    public interface IEnterFieldEvent : ISourceEvent
    {
        FieldSlot Slot { get; }
    }
}
