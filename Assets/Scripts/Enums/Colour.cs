using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum Colour
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Purple = 8,
        Prismatic = 16,

        NonRed = Green | Blue | Purple | Prismatic,
        NonGreen = Red | Blue | Purple | Prismatic,
        NonBlue = Red | Green | Purple | Prismatic,
        NonPurple = Red | Green | Blue | Prismatic,
        NonPrismatic = Red | Green | Blue | Purple,
        Any = Red | Green | Blue | Purple | Prismatic
    }
}
