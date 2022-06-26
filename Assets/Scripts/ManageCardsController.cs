using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManageCardsController : BaseManagerController<CardData>
    {
        private Dropdown ColourDropdown { get; set; }
        private Dropdown CostDropdown { get; set; }
        private Dropdown CardTypeDropdown { get; set; }
        private Dropdown SubTypeDropdown { get; set; }
        private Dropdown SymbolDropdown { get; set; }
        private Dropdown AttackDropdown { get; set; }
        private Dropdown DefenceDropdown { get; set; }
        private Toggle HasPersistenceToggle { get; set; }
        private Toggle IsUniqueToggle { get; set; }


        public override void Start()
        {
            ColourDropdown = GameObject.Find("Canvas/ColourDropdown").GetComponent<Dropdown>();
            ColourDropdown.AddOptions(GetEnumNames(typeof(Colour)));

            CostDropdown = GameObject.Find("Canvas/CostDropdown").GetComponent<Dropdown>();
            CostDropdown.AddOptions(GetRangeText(0, 3));

            CardTypeDropdown = GameObject.Find("Canvas/CardTypeDropdown").GetComponent<Dropdown>();
            CardTypeDropdown.AddOptions(GetEnumNames(typeof(CardType)));

            SubTypeDropdown = GameObject.Find("Canvas/SubTypeDropdown").GetComponent<Dropdown>();
            SubTypeDropdown.AddOptions(GetEnumNames(typeof(SubType)));

            SymbolDropdown = GameObject.Find("Canvas/SymbolDropdown").GetComponent<Dropdown>();
            SymbolDropdown.AddOptions(GetSymbolNames());

            AttackDropdown = GameObject.Find("Canvas/AttackDropdown").GetComponent<Dropdown>();
            AttackDropdown.AddOptions(GetRangeText(0, 20));

            DefenceDropdown = GameObject.Find("Canvas/DefenceDropdown").GetComponent<Dropdown>();
            DefenceDropdown.AddOptions(GetRangeText(0, 20));

            HasPersistenceToggle = GameObject.Find("Canvas/HasPersistenceToggle").GetComponent<Toggle>();
            IsUniqueToggle = GameObject.Find("Canvas/IsUniqueToggle").GetComponent<Toggle>();

            base.Start();
        }

        private List<string> GetSymbolNames()
        {
            const string SymbolsFolder = "Assets/Resources/Sprites/Symbols";

            return Directory
                .GetFiles(SymbolsFolder, "*.png")
                .Select(Path.GetFileName)
                .Where(x => x.Contains("Symbol"))
                .Select(x => x.Substring(0, x.Length - 10))
                .ToList();
        }

        protected override void Submit(CardData data)
        {
            // Retreive inherited properties from UI
            data.Colour = (Colour)Enum.Parse(typeof(Colour), ColourDropdown.captionText.text);
            data.Cost = int.Parse(CostDropdown.captionText.text);

            data.CardType = (CardType)Enum.Parse(typeof(CardType), CardTypeDropdown.captionText.text);
            data.SubTypes = (SubType)Enum.Parse(typeof(SubType), SubTypeDropdown.captionText.text);

            data.Symbol = SymbolDropdown.captionText.text;

            data.Attack = int.Parse(AttackDropdown.captionText.text);
            data.Defence = int.Parse(DefenceDropdown.captionText.text);

            data.HasPersistence = HasPersistenceToggle.isOn;
            data.IsUnique = IsUniqueToggle.isOn;

            base.Submit(data);
        }

        protected override void PopulateComponents(CardData card)
        {
            ColourDropdown.value = ColourDropdown.options.FindIndex(x => x.text == card.Colour.ToString());
            CostDropdown.value = CostDropdown.options.FindIndex(x => x.text == card.Cost.ToString());
            CardTypeDropdown.value = CardTypeDropdown.options.FindIndex(x => x.text == card.CardType.ToString());
            SubTypeDropdown.value = SubTypeDropdown.options.FindIndex(x => x.text == card.SubTypes.ToString());
            SymbolDropdown.value = SymbolDropdown.options.FindIndex(x => x.text == card.Symbol?.ToString());

            AttackDropdown.value = AttackDropdown.options.FindIndex(x => x.text == card.Attack.ToString());
            DefenceDropdown.value = DefenceDropdown.options.FindIndex(x => x.text == card.Defence.ToString());
            HasPersistenceToggle.isOn = card.HasPersistence;
            IsUniqueToggle.isOn = card.IsUnique;

            base.PopulateComponents(card);
        }
    }
}
