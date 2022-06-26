using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.CardEffects
{
    public class ShardOfArah : BaseSourceEffect<ActionCard>
    {
        public override int Id => 7;

        public override IEnumerable<BaseEvent> GetEffect(ActionCard source, BoardState board)
        {
            yield return new DamageTargetsEvent<ActionCard>(source, new TargetConditions(), 1, 3);
            yield return new StrengthenTargetsEvent<ActionCard>(source, new TargetConditions(), 1, 3);
        }
    }

    public class TempestCup : BaseSourceEffect<ActionCard>
    {
        public override int Id => 8;

        public override IEnumerable<BaseEvent> GetEffect(ActionCard source, BoardState board)
        {
            var playerType = board.GetSourceOwner(source).PlayerType;

            yield return new AddTokensEvent(playerType, TokenType.Claw, 3);
            yield return new AddTokensEvent(playerType, TokenType.Cracked, 2);
            yield return new AddLifePlayerEvent(playerType, 5);
        }
    }

    public class WillingSacrifice : CustomAllCreaturesEffect<ActionCard>
    {
        public override int Id => 9;

        public override IEnumerable<BaseEvent> GetEffect(ActionCard source, BoardState board)
        {
            var destroyEvent = new DestroyTargetsEvent<ActionCard, CreatureCard>(
                source,
                new TargetConditions
                {
                    PlayerType = board.GetSourceOwner(source).PlayerType,
                    CardType = CardType.Creature,
                }, 
                1);

            return new List<BaseEvent>() { destroyEvent }.Concat(base.GetEffect(source, board));
        }

        protected override IEnumerable<BaseEvent> Effect(ActionCard source, CreatureCard target)
        {
            yield return new StrengthenCreatureEvent<ActionCard>(source, target, 3);
            yield return new FortifyCreatureEvent<ActionCard>(source, target, 3);
        }

        protected override TargetConditions GetTargetConditions(ActionCard source, BoardState board) => new()
        {
            PlayerType = board.GetSourceOwner(source).PlayerType,
            CardType = CardType.Creature
        };
    }

    public class RollingThunder : BaseSourceEffect<ActionCard>
    {
        public override int Id => 10;

        public override IEnumerable<BaseEvent> GetEffect(ActionCard source, BoardState board)
        {
            yield return new CustomAllCreaturesEvent<ActionCard>(
                source,
                new() { PlayerType = board.GetSourceOwner(source).PlayerType.Opposite() },
                (source, target) => new[] { new DamageCreatureEvent<ActionCard>(source, target, 3) });

            yield return new CustomAllCreaturesEvent<ActionCard>(
                source,
                new() { PlayerType = board.GetSourceOwner(source).PlayerType },
                (source, target) => new[] { new DamageCreatureEvent<ActionCard>(source, target, 1) });
        }
    }
}
