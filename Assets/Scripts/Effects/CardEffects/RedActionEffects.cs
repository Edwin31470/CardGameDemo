using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;

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
            var player = board.GetSourceOwner(source);

            yield return new AddTokensEvent(player, TokenType.Claw, 3);
            yield return new AddTokensEvent(player, TokenType.Cracked, 2);
            yield return new AddLifePlayerEvent(player, 5);
        }
    }

    public class WillingSacrifice : CustomAllCreaturesEffect<ActionCard>
    {
        public override int Id => 9;

        public override IEnumerable<BaseEvent> GetEffect(ActionCard source, BoardState board)
        {
            var destroyTargetEvent = new DestroyTargetsEvent<ActionCard, CreatureCard>(
                source,
                new TargetConditions
                {
                    PlayerType = board.GetSourceOwner(source).PlayerType,
                    CardType = CardType.Creature,
                }, 
                1);

            yield return destroyTargetEvent;
            yield return new CustomTriggerEvent<ActionCard>(
                source,
                (source, board, triggeringEvent) =>
                {
                    return triggeringEvent is IDestroyCardEvent destroyEvent;
                },
                (source, board, triggeringEvent) =>
                {
                    return base.GetEffect(source, board);
                });
        }

        protected override IEnumerable<BaseEvent> Effect(ActionCard source, CreatureCard target, BoardState board)
        {
            // Prevents boosting the destroyed card
            if (!board.GetSourceOwner(target).IsOnField(target))
                yield break;

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
                (source, target, board) => new[] { new DamageCreatureEvent<ActionCard>(source, target, 3) });

            yield return new CustomAllCreaturesEvent<ActionCard>(
                source,
                new() { PlayerType = board.GetSourceOwner(source).PlayerType },
                (source, target, board) => new[] { new DamageCreatureEvent<ActionCard>(source, target, 1) });
        }
    }
}
