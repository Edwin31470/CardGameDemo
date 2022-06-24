using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum SubType
    {
        None = 0,
        Elemental = 1 << 0,
        Undead = 1 << 1,
        Reptile = 1 << 2,
        Bird = 1 << 3,
        Insect = 1 << 4,
        Human = 1 << 5,
        Behemoth = 1 << 6,
        Horror = 1 << 7,
        Immortal = 1 << 8,
        Construct = 1 << 9,
        // Max is 31

        Any = None | Elemental | Undead | Reptile | Bird | Insect | Human | Behemoth | Horror | Immortal | Construct
    }
}
