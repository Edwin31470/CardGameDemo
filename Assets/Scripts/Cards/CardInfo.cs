using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Newtonsoft.Json;

namespace Assets.Scripts.Cards
{
    public class CardInfo
    {
        public CardData CardData { get; set; }
        public Func<BaseCard, IEnumerable<BaseEvent>> GenerateEvents { get; set; }
        public bool IsSummoned { get; set; }
    }
}
