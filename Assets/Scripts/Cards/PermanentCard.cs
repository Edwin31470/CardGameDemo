﻿using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using System.Linq;

namespace Assets.Scripts.Cards
{
    public class PermanentCard : FieldCard
    {
        public PermanentCard(CardInfo cardInfo) : base(cardInfo)
        {

        }

        public override CardType Type => CardType.Permanent;
    }
}
