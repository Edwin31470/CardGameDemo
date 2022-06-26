using Assets.Scripts;
using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManageItemsController : BaseManagerController<ItemData>
    {
        private Dropdown ItemTypeDropdown { get; set; }
        private Toggle IsLegendaryToggle { get; set; }

        public override void Start()
        {
            ItemTypeDropdown = GameObject.Find("Canvas/ItemTypeDropdown").GetComponent<Dropdown>();
            ItemTypeDropdown.AddOptions(GetEnumNames(typeof(ItemType)));

            IsLegendaryToggle = GameObject.Find("Canvas/IsLegendaryToggle").GetComponent<Toggle>();

            base.Start();
        }

        protected override void Submit(ItemData data)
        {
            // Retreive inherited properties from UI
            data.ItemType = (ItemType)Enum.Parse(typeof(ItemType), ItemTypeDropdown.captionText.text);

            data.IsLegendary = IsLegendaryToggle.isOn;

            base.Submit(data);
        }


        protected override void PopulateComponents(ItemData item)
        {
            ItemTypeDropdown.value = ItemTypeDropdown.options.FindIndex(x => x.text == item.ItemType.ToString());
            IsLegendaryToggle.isOn = item.IsLegendary;

            base.PopulateComponents(item);
        }
    }
}

