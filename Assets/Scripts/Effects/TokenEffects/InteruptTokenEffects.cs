using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.TokenEffects
{

    public class ShieldToken : CustomInteruptEffect<BaseToken>
    {
        public override int Id => 7;
        protected override bool TriggerOnce => true;

        protected override bool TryInterupt(BaseToken source, BoardState boardState, IInteruptableEvent interuptedEvent)
        {
            if (interuptedEvent is IDestroyCardEvent destroyCardEvent &&
                boardState.CurrentPhase == Phase.Play &&
                boardState.GetSourceOwner(destroyCardEvent.BaseSource) == boardState.GetSourceOwner(source))
            {
                destroyCardEvent.IsPrevented = true;
                if (destroyCardEvent.BaseSource is CreatureCard creatureCard)
                {
                    creatureCard.BaseDefence.Set(1);
                }

                return true;
            }

            return false;
        }
    }
}
