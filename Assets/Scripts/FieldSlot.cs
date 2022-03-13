using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public class FieldSlot
    {
        public FieldCard Card { get; set; }

        public EffectType EffectType { get; set; }
        private TriggerType TriggerType { get; set; }
        private bool TriggerOnce { get; set; }
        private Func<FieldCard, IEnumerable<BaseEvent>> SlotEffect { get; set; }

        public IEnumerable<BaseEvent> Remove()
        {
            if (TriggerType.HasFlag(TriggerType.Leave)) {
                return Trigger();
            }

            return Enumerable.Empty<BaseEvent>();
        }

        public IEnumerable<BaseEvent> Add(FieldCard card)
        {
            Card = card;

            if (TriggerType.HasFlag(TriggerType.Enter)) {
                return Trigger();
            }

            return Enumerable.Empty<BaseEvent>();
        }

        private IEnumerable<BaseEvent> Trigger()
        {
            var newEvents = SlotEffect.Invoke(Card);

            if (TriggerOnce) {
                TriggerType = TriggerType.None;
                SlotEffect = null;
            }

            return newEvents;
        }

        public void SetEffect(Func<FieldCard, IEnumerable<BaseEvent>> effect, TriggerType triggerType, EffectType type)
        {
            SlotEffect = effect;
            TriggerType = triggerType;
            EffectType = type;
        }
    }
}
