using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum PlayerType
    {
        None = 0,
        Front = 1,
        Back = 2,
        Both = 4,
        Either = Front | Back
    }
}
