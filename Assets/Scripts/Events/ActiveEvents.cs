using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    // Events that trigger once

    public abstract class BaseActiveEvent<T> : BaseBoardEvent where T : BaseSource
    {
        protected T Source { get; set; }

        public BaseActiveEvent(T source)
        {
            Source = source;
        }
    }

    public abstract class BaseStatEvent : BaseActiveEvent<CreatureCard>
    {
        public int Value { get; set; }
        public Stat Stat { get; set; }

        protected BaseStatEvent(CreatureCard source, int value, Stat stat) : base(source)
        {
            Value = value;
            Stat = stat;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Stat.Add(Value);
            yield break;
        }
    }

    public class DamageCreatureEvent : BaseStatEvent
    {

        public DamageCreatureEvent(CreatureCard card, int value) : base(card, -value, card.BaseDefence)
        {
        }
    }

    public class FortifyCreatureEvent : BaseStatEvent
    {
        public FortifyCreatureEvent(CreatureCard card, int value) : base(card, value, card.BaseDefence)
        {
        }
    }

    public class WeakenCreatureEvent : BaseStatEvent
    {
        public WeakenCreatureEvent(CreatureCard card, int value) : base(card, -value, card.BaseAttack)
        {
        }
    }

    public class StrengthenCreatureEvent : BaseStatEvent
    {
        public StrengthenCreatureEvent(CreatureCard card, int value) : base(card, value, card.BaseAttack)
        {
        }
    }

    // TODO: not working (is it needed)
    public class CustomActiveEvent<T> : BaseActiveEvent<T> where T : BaseCard
    {
        public Func<IEnumerable<BaseEvent>> Func { get; set; }

        public CustomActiveEvent(T source, Func<IEnumerable<BaseEvent>> func) : base(source)
        {
            Func = func;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            return Func.Invoke();
        }
    }
}
