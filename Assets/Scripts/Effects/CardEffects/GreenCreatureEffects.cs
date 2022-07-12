using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Effects.CardEffects
{
    public class BirdOfParadise : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 12;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            var player = board.GetSourceOwner(source);

            yield return new AddManaEvent(player, Colour.Green, 1);
        }
    }

    public class ChromaticBasilisk : BaseTargetingEffect<CreatureCard>
    {
        public override int Id => 13;

        protected override TargetConditions GetTargetConditions(CreatureCard source, BoardState boardState) => new()
        {
            CardType = CardType.Creature,
            SubType = SubType.Human
        };

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            yield return new DamageTargetsEvent<CreatureCard>(source, new(), 1, 0);
        }
    }

    public class SteadfastBehemoth: CustomInteruptEffect<CreatureCard>
    {
        public override int Id => 14;

        public override bool TryInterupt(CreatureCard source, BoardState boardState, BaseEvent triggeringEvent)
        {
            if (triggeringEvent is IDamageEvent damageEvent && damageEvent.Target == source)
            {
                damageEvent.Value = 0;
                return true;
            }

            return false;
        }
    }

    public class TurtleshieldVanguard : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 15;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            var player = board.GetSourceOwner(source);

            yield return new AddTokensEvent(player, TokenType.Shell, 2);
        }
    }

    public class BrideOfTheForest : CustomSingleTargetEffect<CreatureCard, CreatureCard>
    {
        public override int Id => 16;

        protected override SelectionType SelectionType => SelectionType.NeutralHighlight;

        protected override string Message => "Charm an opponent’s creature with 8 or less defence.";

        protected override TargetConditions GetTargetConditions(CreatureCard source, BoardState boardState) => new()
        {
            PlayerType = boardState.GetSourceOwner(source).PlayerType.Opposite(),
            CardType = CardType.Creature,
            MaxDefence = 8
        };

        protected override IEnumerable<BaseEvent> OnTargetChosen(CreatureCard source, BoardState boardState, CreatureCard target)
        {
            var player = boardState.GetSourceOwner(source);

            yield return new GainControlOfFieldCardEvent(player, target);
        }
    }

    public class TailwingMoth : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 17;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            var player = board.GetSourceOwner(source);

            yield return new AddLifePlayerEvent(player, 5);
        }
    }

    public class ArishiKingOfLeaves : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 18;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            var player = board.GetSourceOwner(source);

            var greenMana = player.GetManaAmount(Colour.Green);

            yield return new StrengthenCreatureEvent<CreatureCard>(source, source, greenMana * 2);
            yield return new FortifyCreatureEvent<CreatureCard>(source, source, greenMana * 2);
        }
    }
}
