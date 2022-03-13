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
        public Func<IEnumerable<BaseEvent>> EffectEvents { get; set; }
        public bool IsSummoned { get; set; }
    }
}
