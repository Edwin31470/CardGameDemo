using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.ItemEffects
{
    public class PaddedGreaves : CustomInteruptEffect<Item>
    {
        public override int Id => 3;

        protected override bool TryInterupt(Item source, BoardState boardState, IInteruptableEvent interuptableEvent)
        {
            if (interuptableEvent is not IDamageEvent damageEvent)
                return false;

            var itemOwner = boardState.GetSourceOwner(source);
            var damageSourceOwner = boardState.GetSourceOwner(damageEvent.BaseSource);
            var damageTarget = boardState.GetSourceOwner(damageEvent.Target);

            if (itemOwner == damageSourceOwner && itemOwner == damageTarget)
            {
                if (FlipCoin.Flip)
                {
                    damageEvent.Value = 0;
                }

                return true;
            }

            return false;
        }
    }

    public class SpikedFrills : CustomTriggerEffect<Item>
    {
        public override int Id => 4;

        protected override bool Conditions(Item source, BoardState boardState, ITriggeringEvent triggeringEvent)
        {
            return triggeringEvent is DamagePlayerEvent damagePlayerEvent &&
                damagePlayerEvent.Source == boardState.GetSourceOwner(source) &&
                damagePlayerEvent.Value > 10;
        }

        protected override IEnumerable<BaseEvent> OnTrigger(Item source, BoardState boardState, ITriggeringEvent triggeringEvent)
        {
            var damageEvent = (DamagePlayerEvent)triggeringEvent;

            var otherPlayer = boardState.GetPlayer(damageEvent.Source.PlayerType);
            var damageToDo = damageEvent.Value;

            yield return new MessageEvent($"Spiked Frills damages {otherPlayer.PlayerType} Player by {damageToDo}");
            yield return new DamagePlayerEvent(otherPlayer, damageToDo);
        }
    }
}
