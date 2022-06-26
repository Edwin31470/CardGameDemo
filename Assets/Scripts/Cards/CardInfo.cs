using System;
using System.Collections.Generic;
using Assets.Scripts.Effects;
using Assets.Scripts.Events;
using Assets.Scripts.IO;

namespace Assets.Scripts.Cards
{
    public class CardInfo
    {
        public CardData CardData { get; set; }
        public BaseEffect Effect { get; set; }
        //public Func<BaseCard, BoardState, IEnumerable<BaseEvent>> GenerateEvents { get; set; }
        public bool IsSummoned { get; set; }
    }
}
