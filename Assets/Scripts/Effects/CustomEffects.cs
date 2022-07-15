using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Effects
{
    // Effects that generate complex/non-preexisting effects
    // Usually inherits from BaseCardEffect or a SimpleEffect

    // A passive effect with a source and many valid targets
    public abstract class CustomPassiveAllEffect<TSource, TTarget> : BaseTargetingEffect<TSource>
        where TSource : BaseSource
        where TTarget : ITargetable
    {
        public override IEnumerable<BaseEvent> GetEffect(TSource source, BoardState board)
        {
            yield return new CustomPassiveAllEvent<TSource, TTarget>(source, GetTargetConditions(source, board), Effect);
        }

        protected abstract void Effect(TSource source, BoardState boardState, IEnumerable<TTarget> targets);
    }

    // Apply an effect to a single target
    public abstract class CustomSingleTargetEffect<TSource, TTarget> : BaseTargetingEffect<TSource>
        where TSource : BaseSource
        where TTarget : ITargetable
    {
        protected abstract SelectionType SelectionType { get; }
        protected abstract string Message { get; }

        public override IEnumerable<BaseEvent> GetEffect(TSource source, BoardState board)
        {
            yield return new CustomSingleTargetEvent<TSource, TTarget>(source, GetTargetConditions(source, board), OnTargetChosen, SelectionType, Message);
        }

        protected abstract IEnumerable<BaseEvent> OnTargetChosen(TSource source, BoardState boardState, TTarget target);
    }

    // Apply an effect when the source is destroyed
    public abstract class CustomOnDestroyedEffect<T> : BaseSourceEffect<T> where T : BaseCard
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new OnDestroyedEvent<T>(source, Effect);
        }

        protected abstract IEnumerable<BaseEvent> Effect(T source, BoardState boardState);
    }

    // Apply the same effect to each valid target
    public abstract class CustomAllCreaturesEffect<T> : BaseTargetingEffect<T> where T : BaseCard
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomAllCreaturesEvent<T>(source, GetTargetConditions(source, board), Effect);
        }

        protected abstract IEnumerable<BaseEvent> Effect(T source, CreatureCard target, BoardState board);
    }

    // Apply one effect on one trigger definition (default more than once)
    public abstract class CustomInteruptEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        protected virtual bool TriggerOnce => false;

        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomInteruptEvent<T>(source, TryInterupt, TriggerOnce);
        }

        protected abstract bool TryInterupt(T source, BoardState boardState, IInteruptableEvent interuptedEvent);
    }

    // Apply one effect on one trigger definition (default more than once)
    public abstract class CustomTriggerEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        protected virtual bool TriggerOnce => false;

        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomTriggerEvent<T>(source, Conditions, OnTrigger, TriggerOnce);
        }

        protected abstract bool Conditions(T source, BoardState boardState, ITriggeringEvent triggeringEvent);
        protected abstract IEnumerable<BaseEvent> OnTrigger(T source, BoardState boardState, ITriggeringEvent triggeringEvent);
    }

    // Adds events returned by OnGameStart to the queues at the start of every round
    public abstract class CustomOnGameStartEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomOnGameStartEvent<T>(source, OnGameStart);
        }

        protected abstract IEnumerable<BaseEvent> OnGameStart(T source, BoardState boardState, ITriggeringEvent triggeringEvent);
    }

    // Adds events returned by OnRoundStart to the queues at the start of every round
    public abstract class CustomOnRoundStartEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomOnRoundStartEvent<T>(source, OnRoundStart);
        }

        protected abstract IEnumerable<BaseEvent> OnRoundStart(T source, BoardState boardState, ITriggeringEvent roundStartEvent);
    }
}
