using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
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
    public abstract class CustomSingleTargetEffect<T> : BaseTargetingEffect<T> where T : BaseCard
    {
        protected abstract SelectionType SelectionType { get; }
        protected abstract string Message { get; }


        public override IEnumerable<BaseEvent> GetEffect(T source, BoardState board)
        {
            yield return new CustomSingleTargetEvent<T>(source, GetTargetConditions(source, board), Effect, SelectionType, Message);
        }

        protected abstract IEnumerable<BaseEvent> Effect(T source, BoardState boardState, BaseCard target);
    }

    // Apply an effect when the source is destroyed
    public abstract class CustomOnDestroyedEffect<T> : BaseCardEffect<T> where T : BaseCard
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

        protected abstract IEnumerable<BaseEvent> Effect(T source, CreatureCard target);
    }
}
