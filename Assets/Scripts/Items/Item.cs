using Assets.Scripts.Bases;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Items
{
    public class Item : BaseEffectSource
    {
        public string Name { get; set; }
        public ItemType Type { get; }
        public string EffectText { get; set; }
        public string FlavourText { get; set; }
        public bool IsLegendary { get; set; }

        protected Item(ItemInfo itemInfo)
        {
            Id = itemInfo.ItemData.Id;
            Name = itemInfo.ItemData.Name;
            EffectText = itemInfo.ItemData.EffectText;
            FlavourText = itemInfo.ItemData.FlavourText;
            Type = itemInfo.ItemData.ItemType;
            IsLegendary = itemInfo.ItemData.IsLegendary;

            Effect = itemInfo.Effect;
        }

        // Factory method to create
        public static Item Create(ItemInfo itemInfo)
        {
            return new Item(itemInfo);
        }
    }
}
