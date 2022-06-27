using Assets.Scripts.Effects;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Bases
{
    // A source is anything that can be the source of an event (players, cards, items, abilties)
    public abstract class BaseSource
    {

    }

    // Cards, items and abilities have effects
    public abstract class BaseEffectSource : BaseSource
    {
        public BaseEffect Effect { get; set; }

        public IEnumerable<BaseEvent> GetEvents(BoardState board)
        {
            return Effect?.GenerateEffects(this, board) ?? Enumerable.Empty<BaseEvent>();
        }
    }
}
