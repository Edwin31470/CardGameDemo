using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public abstract class BaseActiveEvent : BaseGameplayEvent
    {
    }

    public abstract class BaseStatEvent : BaseActiveEvent
    {
        public CreatureCard Card { get; set; }
        public int Value { get; set; }

        protected BaseStatEvent(CreatureCard card, int value)
        {
            Card = card;
            Value = value;
        }
    }

    public class DamageCreatureEvent : BaseStatEvent
    {

        public DamageCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }

        public override void Process()
        {
            Card.BaseDefence.Remove(Value);
        }
    }

    public class FortifyCreatureEvent : BaseStatEvent
    {
        public FortifyCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override void Process()
        {
            Card.BaseDefence.Add(Value);
        }
    }

    public class WeakenCreatureEvent : BaseStatEvent
    {
        public WeakenCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override void Process()
        {
            Card.BaseAttack.Remove(Value);
        }
    }

    public class StrengthenCreatureEvent : BaseStatEvent
    {
        public StrengthenCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override void Process()
        {
            Card.BaseAttack.Add(Value);
        }
    }

    public class SummonCardEvent : BaseActiveEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Summoing {SummonedCard.Name}";

        private BaseCard SummonedCard { get; set; }

        public SummonCardEvent(CardInfo cardInfo, PlayerType playerType)
        {
            cardInfo.IsSummoned = true;

            switch (cardInfo.CardType)
            {
                case CardType.Creature:
                    SummonedCard = new CreatureCard(playerType, cardInfo);
                    break;
                case CardType.Action:
                    SummonedCard = new ActionCard(playerType, cardInfo);
                    break;
                case CardType.Permanent:
                    SummonedCard = new PermanentCard(playerType, cardInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cardInfo.Type", $"Type must be {CardType.Creature}, {CardType.Action} or {CardType.Permanent}");
            }
        }

        public override void Process()
        {
            if (SummonedCard.Type != CardType.Action)
                MainController.AddEvent(new CreateCardInSlotUIEvent(SummonedCard));

            MainController.AddEvent(new EnterFieldEvent(SummonedCard));
        }
    }

    public class GainControlEvent : BaseActiveEvent
    {
        private BaseCard Card { get; set; }

        public GainControlEvent(BaseCard baseCard)
        {
            Card = baseCard;
        }

        public override void Process()
        {
            var originalPlayer = MainController.GetPlayer(Card.Owner);
            originalPlayer.RemoveFromField(Card);

            Card.Owner = Card.Owner.GetOpposite();

            var newPlayer = MainController.GetPlayer(Card.Owner);
            newPlayer.AddToField(Card);

            MainController.AddEvent(new MoveCardToFieldUIEvent(Card));
        }
    }

    public class CustomActiveEvent : BaseActiveEvent
    {
        public BaseCard Card { get; set; }
        public Action Action { get; set; }

        public CustomActiveEvent(BaseCard baseCard, Action action)
        {
            Card = baseCard;
            Action = action;
        }

        public override void Process()
        {
            Action.Invoke();
        }
    }
}
