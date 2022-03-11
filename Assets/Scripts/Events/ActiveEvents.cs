using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;

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

        public override IEnumerable<BaseEvent> Process()
        {
            Card.BaseDefence.Remove(Value);
            yield break;
        }
    }

    public class FortifyCreatureEvent : BaseStatEvent
    {
        public FortifyCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override IEnumerable<BaseEvent> Process()
        {
            Card.BaseDefence.Add(Value);
            yield break;
        }
    }

    public class WeakenCreatureEvent : BaseStatEvent
    {
        public WeakenCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override IEnumerable<BaseEvent> Process()
        {
            Card.BaseAttack.Remove(Value);
            yield break;
        }
    }

    public class StrengthenCreatureEvent : BaseStatEvent
    {
        public StrengthenCreatureEvent(CreatureCard card, int value) : base(card, value)
        {
        }


        public override IEnumerable<BaseEvent> Process()
        {
            Card.BaseAttack.Add(Value);
            yield break;
        }
    }

    public class CustomActiveEvent : BaseActiveEvent
    {
        public BaseCard Card { get; set; }
        public Func<IEnumerable<BaseEvent>> Func { get; set; }

        public CustomActiveEvent(BaseCard baseCard, Func<IEnumerable<BaseEvent>> func)
        {
            Card = baseCard;
            Func = func;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            return Func.Invoke();
        }
    }
}
