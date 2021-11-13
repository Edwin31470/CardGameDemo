using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public abstract class BaseAllEvent : BaseCardEvent
    {
        protected TargetConditions TargetConditions { get; set; }

        public BaseAllEvent(TargetConditions targetConditions)
        {
            TargetConditions = targetConditions;
        }
    }

    // Targets all field creatures
    public class CustomAllCreaturesEvent : BaseAllEvent
    {
        Action<IEnumerable<CreatureCard>> Action { get; set; }

        public CustomAllCreaturesEvent(TargetConditions targetConditions, Action<IEnumerable<CreatureCard>> action) : base(targetConditions)
        {
            targetConditions.CardType = CardType.Creature;
            Action = action;
        }

        public override void Process()
        {
            var cards = MainController.GetCardsInPlay<CreatureCard>(TargetConditions);
            Action.Invoke(cards);
        }
    }
}
