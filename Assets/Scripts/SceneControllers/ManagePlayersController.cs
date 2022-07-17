using Assets.Scripts;
using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManagePlayersController : MonoBehaviour
    {
        private Dropdown PlayerTypeDropdown { get; set; }
        private Dropdown DeckDropdown { get; set; }
        private Dropdown WeaponDropdown { get; set; }
        private Dropdown ArmourDropdown { get; set; }
        private Dropdown AccessoryDropdown { get; set; }

        private Text WeaponText { get; set; }
        private Text ArmourText { get; set; }
        private Text AccessoryText { get; set; }

        private List<ItemData> Items { get; set; }

        public void Start()
        {
            PlayerTypeDropdown = GameObject.Find("Canvas/PlayerTypeDropdown").GetComponent<Dropdown>();
            PlayerTypeDropdown.AddOptions(new List<string>() { "Front", "Back" });
            PlayerTypeDropdown.onValueChanged.AddListener(x => PopulateWithExisting(x));

            DeckDropdown = GameObject.Find("Canvas/DeckDropdown").GetComponent<Dropdown>();
            DeckDropdown.AddOptions(PlayerIO.ReadDeckNames());

            Items = DataIO.ReadAll<ItemData>().ToList();
            Items.Insert(0, new ItemData
            {
                Id = -1,
                ItemType = ItemType.None,
                Name = "None"
            });

            WeaponDropdown = GameObject.Find("Canvas/WeaponDropdown").GetComponent<Dropdown>();
            WeaponDropdown.AddOptions(Items.Where(x => x.ItemType == ItemType.Weapon || x.ItemType == ItemType.None).Select(x => x.Name).ToList());
            WeaponDropdown.onValueChanged.AddListener(x => PopulateItemText(WeaponDropdown, WeaponText));

            ArmourDropdown = GameObject.Find("Canvas/ArmourDropdown").GetComponent<Dropdown>();
            ArmourDropdown.AddOptions(Items.Where(x => x.ItemType == ItemType.Armour || x.ItemType == ItemType.None).Select(x => x.Name).ToList());
            ArmourDropdown.onValueChanged.AddListener(x => PopulateItemText(ArmourDropdown, ArmourText));

            AccessoryDropdown = GameObject.Find("Canvas/AccessoryDropdown").GetComponent<Dropdown>();
            AccessoryDropdown.AddOptions(Items.Where(x => x.ItemType == ItemType.Accessory || x.ItemType == ItemType.None).Select(x => x.Name).ToList());
            AccessoryDropdown.onValueChanged.AddListener(x => PopulateItemText(AccessoryDropdown, AccessoryText));

            WeaponText = GameObject.Find("Canvas/WeaponText").GetComponent<Text>();
            ArmourText = GameObject.Find("Canvas/ArmourText").GetComponent<Text>();
            AccessoryText = GameObject.Find("Canvas/AccessoryText").GetComponent<Text>();

            // Register button
            GameObject.Find("Canvas/SubmitButton").GetComponent<Button>()
                .onClick.AddListener(() => Submit());

            PopulateWithExisting(0);
        }

        private void PopulateItemText(Dropdown dropdown, Text text)
        {
            var itemName = dropdown.captionText.text;
            var item = Items.Single(x => x.Name == itemName);

            text.text = item.EffectText;
        }

        private void Submit()
        {
            var data = new PlayerData
            {
                PlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), PlayerTypeDropdown.captionText.text),
                DeckName = DeckDropdown.captionText.text,
                WeaponId = Items.Single(x => x.Name == WeaponDropdown.captionText.text).Id,
                ArmourId = Items.Single(x => x.Name == ArmourDropdown.captionText.text).Id,
                AccessoryId = Items.Single(x => x.Name == AccessoryDropdown.captionText.text).Id
            };

            // Write
            PlayerIO.WritePlayer(data);
        }

        // Populate the dropdowns and fields with the data of selected player
        // Front player is 0, Back player is 1
        private void PopulateWithExisting(int player)
        {
            var existing = PlayerIO.ReadPlayer(player == 0 ? PlayerType.Front : PlayerType.Back);
            PopulateComponents(existing);
        }

        // Populate UI with base fields
        private void PopulateComponents(PlayerData data)
        {
            DeckDropdown.value = DeckDropdown.options.FindIndex(x => x.text == data.DeckName.ToString());
            WeaponDropdown.value = data.WeaponId == -1 ? 0 : WeaponDropdown.options.FindIndex(x => x.text == Items[data.WeaponId + 1].Name);
            ArmourDropdown.value = data.ArmourId == -1 ? 0 : ArmourDropdown.options.FindIndex(x => x.text == Items[data.ArmourId + 1].Name);
            AccessoryDropdown.value = data.AccessoryId == -1 ? 0 : AccessoryDropdown.options.FindIndex(x => x.text == Items[data.AccessoryId + 1].Name);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("StartScreen");
            }
        }
    }
}

