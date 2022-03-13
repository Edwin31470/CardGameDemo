
using System;
using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Events
{
    public class BaseSlotEffectEvent : BaseGameplayEvent
    {
    }

    public class SetSlotEffectEvent : BaseSlotEffectEvent
    {
        private Func<FieldCard, IEnumerable<BaseEvent>> Effect { get; set; }
        private FieldSlot Slot { get; set; }
        private TriggerType TriggerType { get; set; }
        private EffectType EffectType { get; set; }

        public SetSlotEffectEvent(Func<FieldCard, IEnumerable<BaseEvent>> effect, FieldSlot slot, TriggerType triggerType, EffectType effectType = EffectType.Neutral)
        {
            Effect = effect;
            Slot = slot;
            TriggerType = triggerType;
            EffectType = effectType;
        }

        public override IEnumerable<BaseEvent> Process()
        { 
            Slot.SetEffect(Effect, TriggerType, EffectType);
            yield return new UpdateSlotGlowUIEvent(Slot, EffectType);
        }
    }
}
