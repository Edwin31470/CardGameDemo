using Assets.Scripts.Cards;
using Assets.Scripts.UI;

namespace Assets.Scripts.Events
{
    public class DrawCardUIEvent : BaseUIEvent
    {
        public BaseCard Card { get; set; }

        public DrawCardUIEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.CreateInHand(Card);
        }
    }

    public class CreateCardInSlotUIEvent : BaseUIEvent
    {
        private FieldCard Card { get; set; }
        private int Index { get; set; }

        public CreateCardInSlotUIEvent(FieldCard card, int index)
        {
            Card = card;
            Index = index;
        }

        public override void Process(UIManager uIManager)
        {
            // TODO: create in index slot instead
            uIManager.CreateInRandomSlot(Card);
        }
    }

    public class MoveCardToFieldUIEvent : BaseUIEvent
    {
        private BaseCard Card { get; set; }
        private int Index { get; set; }

        public MoveCardToFieldUIEvent(BaseCard card, int index)
        {
            Card = card;
            Index = index;
        }

        public override void Process(UIManager uIManager)
        {
            // TODO: Move to index instead
            uIManager.MoveToRandomSlot(Card);
        }
    }

    public class ReturnToDeckUIEvent : BaseUIEvent
    {
        private BaseCard Card { get; set; }

        public ReturnToDeckUIEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            var cardObject = uIManager.GetCardObject(Card);
            uIManager.ReturnToDeck(cardObject);
        }
    }

    public class AddToHandUIEvent : BaseUIEvent
    {
        private BaseCard Card { get; set; }

        public AddToHandUIEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.UpdateHand(Card.Owner.PlayerType);
        }
    }

    public class DestroyCardUIEvent : BaseUIEvent
    {
        private BaseCard Card { get; set; }

        public DestroyCardUIEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            var cardObject = uIManager.GetCardObject(Card);
            uIManager.Destroy(cardObject);
        }
    }
}
