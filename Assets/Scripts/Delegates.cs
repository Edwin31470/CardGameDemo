using System.Collections.Generic;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI;

namespace Assets.Scripts
{
    public delegate IEnumerable<BaseEvent> SelectSingleTarget<TSource, TTarget>(TSource source, BoardState boardState, TTarget target)
        where TSource : BaseSource
        where TTarget : ITargetable;

    public delegate IEnumerable<BaseEvent> SelectMultipleTargets<TSource, TTarget>(TSource source, BoardState boardState, IEnumerable<TTarget> targets)
        where TSource : BaseSource
        where TTarget : ITargetable;

    public delegate IEnumerable<BaseEvent> OnTargetsChosen<T>(IEnumerable<T> targets) where T : ITargetable;

    public delegate void OnFinishTargeting<out T>(IEnumerable<BaseUIObject> targetObjects)
        where T : ITargetable;
}
