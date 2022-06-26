using Assets.Scripts.Effects;
using Assets.Scripts.Events;
using Assets.Scripts.IO;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Managers
{
    public static class ItemManager
    {
        private static readonly Dictionary<int, ItemData> ItemLibrary = DataIO.ReadAll<ItemData>().ToDictionary(x => x.Id, x => x);

        private static readonly Dictionary<int, BaseEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseEffect))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseSourceEffect<Item>)))
            .Select(x => (BaseEffect)Activator.CreateInstance(x))
            .ToDictionary(x => x.Id, x => x);

        public static List<ItemInfo> GetItems(IEnumerable<int> itemIds)
        {
            // eventually will read ids from a player config file

            return itemIds.Select(x => new ItemInfo
            {
                ItemData = ItemLibrary[x],
                Effect = EffectLibrary[x]
            }).ToList();
        }
    }
}
