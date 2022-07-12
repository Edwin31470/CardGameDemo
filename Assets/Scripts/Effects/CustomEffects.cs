using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Effects
{
    // Effects that generate complex/non-preexisting effects
    // Usually inherits from BaseCardEffect or a SimpleEffect

    // A passive effect with a source and many valid targets
    public abstract class CustomPassiveAllCreaturesEffect<T> : BaseTargetingEffect<T> where T : BaseCard
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomPassiveAllCreaturesEvent<T>(source, GetTargetConditions(source, board), Effect);
        }

        protected abstract void Effect(T source, BoardState boardState, IEnumerable<CreatureCard> targets);
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
        public virtual bool TriggerOnce => false;

        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomInteruptEvent<T>(source, TryInterupt, TriggerOnce);
        }

        public abstract bool TryInterupt(T source, BoardState boardState, BaseEvent triggeringEvent);
    }

    // Apply one effect on one trigger definition (default more than once)
    public abstract class CustomTriggerEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        public virtual bool TriggerOnce => false;

        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomTriggerEvent<T>(source, Conditions, OnTrigger, TriggerOnce);
        }

        public abstract bool Conditions(T source, BoardState boardState, BaseEvent triggeringEvent);
        public abstract IEnumerable<BaseEvent> OnTrigger(T source, BoardState boardState, BaseEvent triggeringEvent);
    }

    // Apply one effect at the start of the game
    public abstract class CustomOnGameStartEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomOnGameStartEvent<T>(source, OnGameStart);
        }

        public abstract IEnumerable<BaseEvent> OnGameStart(T source, BoardState boardState, BaseEvent triggeringEvent);
    }

    // Apply one effect on every round start
    public abstract class CustomOnRoundStartEffect<T> : BaseSourceEffect<T> where T : BaseSource
    {
        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomOnRoundStartEvent<T>(source, OnRoundStart);
        }

        public abstract IEnumerable<BaseEvent> OnRoundStart(T source, BoardState boardState, BaseEvent triggeringEvent);
    }
}
