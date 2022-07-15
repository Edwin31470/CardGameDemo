using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.TokenEffects
{
    public class RageToken : CustomTriggerEffect<BaseToken>
    {
        public override int Id => 5;

        protected override bool Conditions(BaseToken source, BoardState boardState, ITriggeringEvent triggeringEvent)
        {
            return triggeringEvent is PlayActionCardEvent playActionEvent && boardState.GetSourceOwner(playActionEvent.Source) != boardState.GetSourceOwner(source);
        }

        protected override IEnumerable<BaseEvent> OnTrigger(BaseToken source, BoardState boardState, ITriggeringEvent triggeringEvent)
        {
            yield return new CustomAllCreaturesEvent<BaseToken>(
                source,
                new() { PlayerType = boardState.GetSourceOwner(source).PlayerType },
                StrengthenCreature);
        }

        private IEnumerable<BaseEvent> StrengthenCreature(BaseToken source, CreatureCard target, BoardState board)
        {
            yield return new StrengthenCreatureEvent<BaseToken>(source, target, 1);
        }
    }

    public class SteadfastToken : CustomTriggerEffect<BaseToken>
    {
        public override int Id => 6;

        protected override bool Conditions(BaseToken source, BoardState board, ITriggeringEvent triggeringEvent)
        {
            return triggeringEvent is PlayActionCardEvent playActionEvent && board.GetSourceOwner(playActionEvent.Source) != board.GetSourceOwner(source);
        }

        protected override IEnumerable<BaseEvent> OnTrigger(BaseToken source, BoardState board, ITriggeringEvent triggeringEvent)
        {
            yield return new CustomAllCreaturesEvent<BaseToken>(
                source,
                new() { PlayerType = board.GetSourceOwner(source).PlayerType },
                FortifyCreature);
        }

        private IEnumerable<BaseEvent> FortifyCreature(BaseToken source, CreatureCard target, BoardState board)
        {
            yield return new FortifyCreatureEvent<BaseToken>(source, target, 1);
        }
    }
}
