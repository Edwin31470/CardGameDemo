using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum TriggerType
    {
        None = 0,
        Enter = 1,
        Leave = 2,
        Trigger = 4,
        EnterAndLeave = Enter | Leave
    }
}
