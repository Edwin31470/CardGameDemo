using System;
using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;

namespace Assets.Scripts.Effects
{
    // Effects are singleton classes that generate a list of events
    // Used for cards, abilities, items and slots

    public abstract class BaseEffect
    {

    }

    public abstract class BaseCardEffect
    {
        public abstract int CardId { get; }
        public abstract IEnumerable<BaseEvent> GetEffect(BaseCard source);

        // Conditional methods
        protected static Func<BaseEvent, bool> IsDestroyed(BaseCard target)
        {
            return triggeringEvent => triggeringEvent is DestroyCardEvent destroyCardEvent && destroyCardEvent.Card == target;
        }
    }
}