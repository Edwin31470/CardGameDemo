using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public delegate IEnumerable<BaseEvent> SelectSingleTarget<T>(T source, BoardState boardState, BaseCard target);

    public delegate IEnumerable<BaseEvent> SelectMultipleTargets<T>(T source, BoardState boardState, IEnumerable<BaseCard> targets);
}
