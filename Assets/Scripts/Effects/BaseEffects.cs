using System;
using System.Collections.Generic;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using Assets.Scripts.Items;

namespace Assets.Scripts.Effects
{
    public static class FlipCoin
    {
        private static readonly Random _random = new((int)DateTime.Now.Ticks);

        public static bool Flip => _random.Next(0, 2) == 0;
    }

    // Effects are singleton classes that generate a list of events
    // Used for cards, abilities, items and slots

    public abstract class BaseEffect
    {
        public abstract int Id { get; }
        public abstract IEnumerable<BaseEvent> GenerateEffects(BaseSource source, BoardState board);
    }

    // An effect with an item source
    public abstract class BaseItemEffect : BaseEffect
    {
        public override IEnumerable<BaseEvent> GenerateEffects(BaseSource source, BoardState board)
        {
            return GenerateEffects(source, board);
        }

        public abstract IEnumerable<BaseEvent> GenerateEffects(BaseItem source, BoardState board);
    }

    // An effect with a card source
    public abstract class BaseCardEffect : BaseEffect
    {
        public override IEnumerable<BaseEvent> GenerateEffects(BaseSource source, BoardState board)
        {
            return GenerateEffects(source, board);
        }

        public abstract IEnumerable<BaseEvent> GenerateEffects(BaseCard source, BoardState board);
    }

    // An effect with a card source type
    public abstract class BaseCardEffect<T> : BaseCardEffect where T : BaseCard
    {
        public abstract IEnumerable<BaseEvent> GetEffect(T source, BoardState board);

        public override IEnumerable<BaseEvent> GenerateEffects(BaseCard source, BoardState board)
        {
            return GetEffect((T)source, board);
        }
    }

    // An effect with targeting
    public abstract class BaseTargetingEffect<T> : BaseCardEffect<T> where T : BaseCard
    {
        protected abstract TargetConditions GetTargetConditions(T source, BoardState board);
    }
}