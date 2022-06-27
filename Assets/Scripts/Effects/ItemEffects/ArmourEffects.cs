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

        public override bool TryInterupt(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            if (triggeringEvent is not IDamageEvent damageEvent)
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

        public override bool Conditions(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is DamagePlayerEvent damagePlayerEvent &&
                damagePlayerEvent.PlayerType == boardState.GetSourceOwner(source).PlayerType &&
                damagePlayerEvent.Value > 10;
        }

        public override IEnumerable<BaseEvent> OnTrigger(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            var damageEvent = (DamagePlayerEvent)triggeringEvent;
            var damage = damageEvent.Value;

            yield return new MessageEvent($"Spiked Frills damages {damageEvent.PlayerType.Opposite()} Player by {damage / 10}");
            yield return new DamagePlayerEvent(damageEvent.PlayerType.Opposite(), damage / 10);
        }
    }
}
