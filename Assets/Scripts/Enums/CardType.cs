using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum CardType
    {
        None = 0,
        Creature = 1,
        Action = 2,
        Permanent = 4,
        FieldCard = Creature | Permanent,
        Any = FieldCard | Action
    }
}
