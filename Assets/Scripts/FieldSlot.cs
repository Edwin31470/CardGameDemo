using System;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;

namespace Assets.Scripts
{
    public class FieldSlot
    {
        public FieldCard Card { get; set; }

        private TriggerType TriggerType { get; set; }
        private bool TriggerOnce { get; set; }
        private Action<FieldCard> SlotEffect { get; set; }

        public FieldCard Take()
        {
            if (TriggerType.HasFlag(TriggerType.Leave)) {
                Trigger();
            }

            var card = Card;
            Card = null;
            return card;
        }

        public void Add(FieldCard card)
        {
            if (TriggerType.HasFlag(TriggerType.Enter)) {
                Trigger();
            }

            Card = card;
        }

        private void Trigger()
        {
            SlotEffect.Invoke(Card);

            if (TriggerOnce)
                TriggerType = TriggerType.None;
        }
    }
}
