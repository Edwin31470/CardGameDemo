﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    //TODO: should be able to use a target event with all cards already targeted
    public abstract class BaseAllEvent : BaseAreaEvent
    {
        protected TargetConditions TargetConditions { get; set; }

        protected BaseAllEvent(TargetConditions targetConditions)
        {
            TargetConditions = targetConditions;
        }
    }

    // Targets all field creatures
    public class CustomAllCreaturesEvent : BaseAllEvent
    {
        Func<IEnumerable<CreatureCard>,IEnumerable<BaseEvent>> Func { get; set; }

        public CustomAllCreaturesEvent(TargetConditions targetConditions, Func<IEnumerable<CreatureCard>,IEnumerable<BaseEvent>> func) : base(targetConditions)
        {
            targetConditions.CardType = CardType.Creature;
            Func = func;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var cards = board.GetMatchingCards(TargetConditions).OfType<CreatureCard>();
            return Func.Invoke(cards);
        }
    }
}
