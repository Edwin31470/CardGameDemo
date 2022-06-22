using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System.Collections.Generic;

namespace Assets.Scripts.Effects
{
    // Effects that generate complex/non-preexisting effects
    // Usually inherits from BaseCardEffect or a SimpleEffect

    public abstract class CustomPassiveCreatureSourceAllCreaturesEffect : BaseCardEffect
    {
        protected abstract TargetConditions TargetConditions { get; }

        public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
        {
            yield return new CustomPassiveCreatureSourceAllCreaturesEvent(source, TargetConditions, Effect);
        }

        protected abstract void Effect(BoardState boardState, CreatureCard source, IEnumerable<CreatureCard> targets);
    }

    public abstract class CustomSingleTargetEffect : SimpleTargetEffect
    {
        protected abstract SelectionType SelectionType { get; }
        protected abstract string Message { get; }


        public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
        {
            yield return new CustomSingleTargetEvent(source, TargetConditions, Effect, SelectionType, Message);
        }

        protected abstract IEnumerable<BaseEvent> Effect(BoardState boardState, BaseCard source, BaseCard target);
    }

    public abstract class CustomOnDestroyedEffect : BaseCardEffect
    {
        public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
        {
            yield return new OnDestroyedEvent(source, Effect);
        }

        protected abstract IEnumerable<BaseEvent> Effect(BoardState boardState, BaseCard source);
    }
}
