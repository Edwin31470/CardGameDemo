using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Effects.CardEffects
{
    public class RoilingElemental : CustomPassiveAllEffect<CreatureCard, CreatureCard>
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

                var player = boardState.GetSourceOwner(source);

                yield return new SummonCardEvent(player, cardInfo);
            }
        }
    }

    public class BoilingElemental : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 2;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new DamageTargetsEvent<CreatureCard>(source, new(), 1, 4);
        }
    }

    public class FlameTongueKijiti : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 3;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new AddTokensEvent(board.GetSourceOwner(source), TokenType.Claw, 2);
        }
    }

    public class SoaringDamned : CustomOnDestroyedEffect<CreatureCard>
    {
        public override int Id => 5;

        protected override IEnumerable<BaseEvent> Effect(CreatureCard source, BoardState boardState)
        {
            yield return new DestroyTargetsEvent<CreatureCard, CreatureCard>(source, new(), 1);
        }
    }

    public class EruptionIdol : CustomAllCreaturesEffect<CreatureCard>
    {
        public override int Id => 6;

        protected override TargetConditions GetTargetConditions(CreatureCard source, BoardState board) => new()
        {
            CardType = CardType.Creature
        };

        protected override IEnumerable<BaseEvent> Effect(CreatureCard source, CreatureCard target, BoardState board)
        {
            if (target == source)
                yield break;

            if (FlipCoin.Flip)
            {
                yield return new DamageCreatureEvent<CreatureCard>(source, target, 5);
            }
            else
            {
                yield return new WeakenCreatureEvent<CreatureCard>(source, target, 5);
            }
        }
    }
}
