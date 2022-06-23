using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    // Active events that apply the same effect to all valid targets

    public abstract class BaseAllEvent<T> : BaseActiveEvent<T> where T : BaseCard
    {
        protected TargetConditions TargetConditions { get; set; }

        protected BaseAllEvent(T source, TargetConditions targetConditions) : base(source)
        {
            TargetConditions = targetConditions;
        }
    }

    // Targets all field creatures
    public class CustomAllCreaturesEvent<T> : BaseAllEvent<T> where T : BaseCard
    {
        private Func<T, CreatureCard, IEnumerable<BaseEvent>> Func { get; set; }

        public CustomAllCreaturesEvent(T source, TargetConditions targetConditions, Func<T, CreatureCard, IEnumerable<BaseEvent>> func) : base(source, targetConditions)
        {
            targetConditions.CardType = CardType.Creature;
            Func = func;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var cards = board.GetMatchingCards(TargetConditions).OfType<CreatureCard>();

            var events = new List<BaseEvent>();
            foreach (var card in cards)
            {
                events.AddRange(Func.Invoke(Source, card));
            }

            return events;
        }
    }
}
