using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Effects
{
    public class RoilingElementalEffect : CustomPassiveCreatureSourceAllCreaturesEffect
    {
        public override int CardId => 0;
        protected override TargetConditions TargetConditions => new()
        {
            CardType = CardType.Creature,
            SubType = SubType.Elemental
        };

        protected override void Effect(BoardState boardState, CreatureCard source, IEnumerable<CreatureCard> targets)
        {
            var count = targets.Count(x => x != source);

            source.BonusAttack.Add(count);
            source.BonusDefence.Add(count);
        }
    }

    public class SmoulderingDraug : CustomOnDestroyedEffect
    {
        public override int CardId => 1;

        public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
        {
            yield return new OnDestroyedEvent(source, Effect);
        }

        protected override IEnumerable<BaseEvent> Effect(BoardState boardState, BaseCard source)
        {
            if (boardState.CurrentPhase == Phase.Play)
            {
                var cardInfo = source.ToCardInfo();
                cardInfo.CardData.Attack = 2;
                cardInfo.CardData.Defence = 2;

                yield return new SummonCardEvent(cardInfo, boardState.GetCardOwner(source).PlayerType);
            }
        }
    }

    public class BoilingElemental : SimpleTargetEffect
    {
        public override int CardId => 2;

        protected override TargetConditions TargetConditions => new()
        {
            CardType = CardType.Creature
        };

        public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
        {
            yield return new DamageTargetsEvent(TargetConditions, 1, 4);
        }
    }
}
