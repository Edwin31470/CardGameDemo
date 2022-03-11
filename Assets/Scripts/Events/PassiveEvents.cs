﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    public abstract class BasePassiveEvent : BaseEvent
    {
        public BaseCard Source { get; set; }

        protected BasePassiveEvent(BaseCard source)
        {
            Source = source;
        }

        public abstract void Process(MainController mainController);

        // Passive events are only valid when their source is on the field
        public bool IsValid()
        {
            return Source.Owner.IsOnField(Source);
        }
    }

    // For events without targets
    public class CustomPassiveEvent : BasePassiveEvent
    {
        private Action Action { get; set; } // Owner - Targets

        public CustomPassiveEvent(BaseCard owner, Action action) : base(owner)
        {
            Action = action;
        }

        public override void Process(MainController mainController)
        {
            Action.Invoke();
        }
    }

    // For events with creature targets
    public class CustomPassiveAllCreaturesEvent : BasePassiveEvent
    {
        private TargetConditions TargetConditions { get; set; } // Filter out targets not matching these conditions
        private Action<HashSet<CreatureCard>> Action { get; set; } // Targets

        public CustomPassiveAllCreaturesEvent(BaseCard owner, TargetConditions targetConditions, Action<HashSet<CreatureCard>> action)
            : base(owner)
        {
            targetConditions.CardType = CardType.Creature;
            TargetConditions = targetConditions;
            Action = action;
        }

        public override void Process(MainController mainController)
        {
            var cards = MainController.GetCardsInPlay<CreatureCard>(TargetConditions);
            Action.Invoke(cards);
        }
    }
}
