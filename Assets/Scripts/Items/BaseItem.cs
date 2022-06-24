using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Items
{
    public class BaseItem : BaseSource
    {
        public string Name { get; set; }
        public ItemType Type { get; }
        public string EffectText { get; set; }
        public string FlavourText { get; set; }
        public bool IsLegendary { get; set; }

        private Func<BaseItem /* this */, BoardState, IEnumerable<BaseEvent>> GenerateEvents { get; set; }

        protected BaseItem(ItemInfo itemInfo)
        {
            Name = itemInfo.ItemData.Name;
            EffectText = itemInfo.ItemData.EffectText;
            FlavourText = itemInfo.ItemData.FlavourText;
            Type = itemInfo.ItemData.ItemType;
            GenerateEvents = itemInfo.GenerateEvents;
            IsLegendary = itemInfo.ItemData.IsLegendary;

            GenerateEvents = itemInfo.GenerateEvents;
        }

        public IEnumerable<BaseEvent> GetEvents(BoardState board)
        {
            return GenerateEvents.Invoke(this, board);
        }

        // Factory method to create
        public static BaseItem Create(ItemInfo itemInfo)
        {
            return new BaseItem(itemInfo);
        }
    }
}
