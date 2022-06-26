﻿using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Events.Interfaces;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    // Events that trigger once

    public abstract class BaseActiveEvent<T> : BaseSourceEvent<T> where T : BaseSource
    {
        public BaseActiveEvent(T source) : base(source)
        {
        }
    }

    // Source is where the damage/etc. is coming from, target is what is being damaged/etc.
    public abstract class BaseStatEvent<T> : BaseActiveEvent<T>, IStatEvent where T : BaseSource
    {
        public CreatureCard Target { get; set; }

        public int Value { get; set; }
        private Action<int> Action { get; set; }

        protected BaseStatEvent(T source, CreatureCard target, int value, Action<int> action) : base(source)
        {
            Target = target;
            Value = value;
            Action = action;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Action.Invoke(Value);
            yield break;
        }
    }

    public class DamageCreatureEvent<T> : BaseStatEvent<T>, IDamageEvent where T : BaseSource
    {
        public DamageCreatureEvent(T source, CreatureCard target, int value) : base(source, target, value, target.BaseDefence.Remove)
        {
        }
    }

    public class FortifyCreatureEvent<T> : BaseStatEvent<T>, IFortifyEvent where T : BaseSource
    {
        public FortifyCreatureEvent(T source, CreatureCard target, int value) : base(source, target, value, target.BaseDefence.Add)
        {
        }
    }

    public class WeakenCreatureEvent<T> : BaseStatEvent<T>, IWeakenEvent where T : BaseSource
    {
        public WeakenCreatureEvent(T source, CreatureCard target, int value) : base(source, target, value, target.BaseAttack.Remove)
        {
        }
    }

    public class StrengthenCreatureEvent<T> : BaseStatEvent<T>, IStrengthenEvent where T : BaseSource
    {
        public StrengthenCreatureEvent(T source, CreatureCard target, int value) : base(source, target, value, target.BaseAttack.Add)
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
