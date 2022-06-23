using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Effects.CardEffects
{
    public class RoilingElemental : CustomPassiveAllCreaturesEffect<CreatureCard>
    {
        public override int Id => 0;
        protected override TargetConditions GetTargetConditions(CreatureCard source, BoardState boardState) => new()
        {
            CardType = CardType.Creature,
            SubType = SubType.Elemental
        };

        protected override void Effect(CreatureCard source, BoardState boardState, IEnumerable<CreatureCard> targets)
        {
            var count = targets.Count(x => x != source);

            source.BonusAttack.Add(count);
            source.BonusDefence.Add(count);
        }
    }

    public class SmoulderingDraug : CustomOnDestroyedEffect<CreatureCard>
    {
        public override int Id => 1;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new OnDestroyedEvent<CreatureCard>(source, Effect);
        }

        protected override IEnumerable<BaseEvent> Effect(CreatureCard source, BoardState boardState)
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

    public class BoilingElemental : BaseCardEffect<CreatureCard>
    {
        public override int Id => 2;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new DamageTargetsEvent(new() { CardType = CardType.Creature }, 1, 4);
        }
    }

    public class FlameTongueKijiti : BaseCardEffect<CreatureCard>
    {
        public override int Id => 3;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new AddTokensEvent(board.GetCardOwner(source).PlayerType, TokenType.Claw, 2);
        }
    }

    public class SoaringDamned : CustomOnDestroyedEffect<CreatureCard>
    {
        public override int Id => 5;

        protected override IEnumerable<BaseEvent> Effect(CreatureCard source, BoardState boardState)
        {
            yield return new DestroyTargetsEvent(new TargetConditions { CardType = CardType.Creature }, 1);
        }
    }

    public class EruptionIdol : CustomAllCreaturesEffect<CreatureCard>
    {
        public override int Id => 6;

        protected override TargetConditions GetTargetConditions(CreatureCard source, BoardState board) => new()
        {
            CardType = CardType.Creature,
            PlayerType = board.GetCardOwner(source).PlayerType,
        };

        protected override IEnumerable<BaseEvent> Effect(CreatureCard source, CreatureCard target)
        {
            if (target == source)
                yield break;

            if (FlipCoin.Flip)
            {
                yield return new DamageCreatureEvent(target, 5);
            }
            else
            {
                yield return new WeakenCreatureEvent(target, 5);
            }
        }
    }
}
