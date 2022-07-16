using Assets.Scripts.Interfaces;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ItemObject : MoveableUIObject
    {
        public Item ItemReference { get; set; }
        public override ITargetable SourceReference => ItemReference;

        private SpriteRenderer ItemSymbol { get; set; }

        public void Initialize(Item itemReference)
        {
            ItemReference = itemReference;

            ItemSymbol = transform.Find("ItemSymbol").GetComponent<SpriteRenderer>();

            base.Initialize();
        }
    }
}
