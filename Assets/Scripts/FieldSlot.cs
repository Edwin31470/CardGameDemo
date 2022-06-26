using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public class FieldSlot : BaseSource
    {
        public FieldCard Card { get; set; }

        public EffectType EffectType { get; set; }
        private TriggerType TriggerType { get; set; }
        private bool TriggerOnce { get; set; }
        private Func<FieldCard, IEnumerable<BaseEvent>> SlotEffect { get; set; }

        public IEnumerable<BaseEvent> Remove()
        {
            var card = Card;
            Card = null;

            if (TriggerType.HasFlag(TriggerType.Leave)) {
                return Trigger(card);
            }

            return Enumerable.Empty<BaseEvent>();
        }

        public IEnumerable<BaseEvent> Add(BaseCard card)
        {
            Card = (FieldCard)card;

            if (TriggerType.HasFlag(TriggerType.Enter)) {
                return Trigger(Card);
            }

            return Enumerable.Empty<BaseEvent>();
        }

        private IEnumerable<BaseEvent> Trigger(FieldCard card)
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
