using System.Collections.Generic;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public delegate IEnumerable<BaseEvent> SelectSingleTarget<TSource, TTarget>(TSource source, BoardState boardState, TTarget target)
        where TSource : BaseSource
        where TTarget : BaseCard;

    public delegate IEnumerable<BaseEvent> SelectMultipleTargets<TSource, TTarget>(TSource source, BoardState boardState, IEnumerable<TTarget> targets)
        where TSource : BaseSource
        where TTarget : BaseCard;

    public delegate IEnumerable<BaseEvent> OnFinishSelection(IEnumerable<BaseCard> targets);
}
