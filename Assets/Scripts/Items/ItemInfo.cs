using Assets.Scripts.Events;
using Assets.Scripts.IO;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Items
{
    public class ItemInfo
    {
        public ItemData ItemData { get; set; }
        public Func<BaseItem, BoardState, IEnumerable<BaseEvent>> GenerateEvents { get; set; }
    }
}
