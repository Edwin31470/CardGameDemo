using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public delegate IEnumerable<BaseEvent> SelectSingleTarget(BoardState boardState, BaseCard source, BaseCard target);

    public delegate IEnumerable<BaseEvent> SelectMultipleTargets(BoardState boardState, BaseCard source, IEnumerable<BaseCard> targets);
}
