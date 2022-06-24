using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public class ItemInfo
    {
        public ItemData ItemData { get; set; }
        public Func<BaseItem, BoardState, IEnumerable<BaseEvent>> GenerateEvents { get; set; }
    }
}
