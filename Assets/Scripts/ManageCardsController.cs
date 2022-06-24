using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManageCardsController : MonoBehaviour
    {
        private InputField NameField { get; set; }
        private Dropdown ColourDropdown { get; set; }
        private Dropdown CostDropdown { get; set; }
        private Dropdown CardTypeDropdown { get; set; }
        private Dropdown SubTypeDropdown { get; set; }
        private Dropdown SymbolDropdown { get; set; }
        private Dropdown AttackDropdown { get; set; }
        private Dropdown DefenceDropdown { get; set; }
        private InputField EffectTextField { get; set; }
        private InputField FlavourTextField { get; set; }
        private Toggle HasPersistenceToggle { get; set; }
        private Toggle IsUniqueToggle { get; set; }


        private Dropdown EditCardDropdown { get; set; }

        void Start()
        {
            NameField = GameObject.Find("Canvas/NameField").GetComponent<InputField>();

            var colourDropdown = GameObject.Find("Canvas/ColourDropdown").GetComponent<Dropdown>();
            colourDropdown.AddOptions(GetEnumNames(typeof(Colour)));
            ColourDropdown = colourDropdown;

            var costDropdown = GameObject.Find("Canvas/CostDropdown").GetComponent<Dropdown>();
            costDropdown.AddOptions(GetRangeText(0, 3));
            CostDropdown = costDropdown;

            var cardTypeDropdown = GameObject.Find("Canvas/CardTypeDropdown").GetComponent<Dropdown>();
            cardTypeDropdown.AddOptions(GetEnumNames(typeof(CardType)));
            CardTypeDropdown = cardTypeDropdown;

            var subTypeDropdown = GameObject.Find("Canvas/SubTypeDropdown").GetComponent<Dropdown>();
            subTypeDropdown.AddOptions(GetEnumNames(typeof(SubType)));
            SubTypeDropdown = subTypeDropdown;

            var symbolDropdown = GameObject.Find("Canvas/SymbolDropdown").GetComponent<Dropdown>();
            symbolDropdown.AddOptions(GetSymbolNames());
            SymbolDropdown = symbolDropdown;

            EffectTextField = GameObject.Find("Canvas/EffectTextField").GetComponent<InputField>();
            FlavourTextField = GameObject.Find("Canvas/FlavourTextField").GetComponent<InputField>();

            var attackDropdown = GameObject.Find("Canvas/AttackDropdown").GetComponent<Dropdown>();
            attackDropdown.AddOptions(GetRangeText(0, 20));
            AttackDropdown = attackDropdown; 
            
            var defenceDropdown = GameObject.Find("Canvas/DefenceDropdown").GetComponent<Dropdown>();
            defenceDropdown.AddOptions(GetRangeText(0, 20));
            DefenceDropdown = defenceDropdown;

            HasPersistenceToggle = GameObject.Find("Canvas/HasPersistenceToggle").GetComponent<Toggle>();
            IsUniqueToggle = GameObject.Find("Canvas/IsUniqueToggle").GetComponent<Toggle>();

            var editCardDropdown = GameObject.Find("Canvas/EditCardDropdown").GetComponent<Dropdown>();
            editCardDropdown.AddOptions(GetExistingCardNames());
            EditCardDropdown = editCardDropdown;
            EditCardDropdown.onValueChanged.AddListener(x => PopulateCardToEdit(x - 1));


            // Register button
            GameObject.Find("Canvas/SubmitButton").GetComponent<Button>()
                .onClick.AddListener(SubmitCard);
        }

        private void SubmitCard()
        {
            var selectedExistingCard = EditCardDropdown.captionText.text;

            int id;
            if (selectedExistingCard == string.Empty)
            {
                var existingCards = CardIO.ReadAll();
                id = existingCards.Max(x => x.Id) + 1;
            }
            else
            {
                id = int.Parse(selectedExistingCard[^2..]);
            }

            var name = NameField.text;
            var colour = (Colour)Enum.Parse(typeof(Colour), ColourDropdown.captionText.text);
            var cost = int.Parse(CostDropdown.captionText.text);

            var cardType = (CardType)Enum.Parse(typeof(CardType), CardTypeDropdown.captionText.text);
            var subType = (SubType)Enum.Parse(typeof(SubType), SubTypeDropdown.captionText.text);

            var symbol = SymbolDropdown.captionText.text;

            var effectText = EffectTextField.text;
            var flavourText = FlavourTextField.text;

            var attack = int.Parse(AttackDropdown.captionText.text);
            var defence = int.Parse(DefenceDropdown.captionText.text);

            var hasPersistence = HasPersistenceToggle.isOn;
            var isUnique = IsUniqueToggle.isOn;

            var cardData = new CardData
            {
                Id = id,
                Name = name,
                Colour = colour,
                Cost = cost,
                CardType = cardType,
                SubTypes = subType,
                Symbol = symbol,
                EffectText = effectText,
                FlavourText = flavourText,
                Attack = attack,
                Defence = defence,
                HasPersistence = hasPersistence,
                IsUnique = isUnique,
            };

            CardIO.WriteCard(cardData);

            EditCardDropdown.ClearOptions();
            EditCardDropdown.AddOptions(GetExistingCardNames());
            PopulateComponents(new CardData());
        }

        private List<string> GetEnumNames(Type enumType)
        {
            if (enumType == typeof(Colour))
            {
                return new List<string>()
                {
                    "None",
                    "Red",
                    "Blue",
                    "Green",
                    "Purple",
                    "Prismatic"
                };
            }

            if (enumType == typeof(CardType))
            {
                return new List<string>()
                {
                    "None",
                    "Creature",
                    "Action",
                    "Permanent"
                };
            }

            return Enum.GetNames(enumType).ToList();
        }

        private List<string> GetRangeText(int min, int max)
        {
            return Enumerable.Range(0, max - min + 1).Select(x => x.ToString()).ToList();
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

        private List<string> GetExistingCardNames()
        {
            var existingCards = CardIO.ReadAll()
                .OrderBy(x => x.Id)
                .Select(x => $"{x.Name} - {x.Id}")
                .ToList();

            existingCards.Insert(0, string.Empty);

            return existingCards;
        }

        private void PopulateCardToEdit(int id)
        {
            if (id == -1)
            {
                PopulateComponents(new CardData());
                return;
            }

            var existingCard = CardIO.ReadCard(id);
            PopulateComponents(existingCard);
        }

        private void PopulateComponents(CardData card)
        {
            NameField.text = card.Name;
            ColourDropdown.value = ColourDropdown.options.FindIndex(x => x.text == card.Colour.ToString());
            CostDropdown.value = CostDropdown.options.FindIndex(x => x.text == card.Cost.ToString());
            CardTypeDropdown.value = CardTypeDropdown.options.FindIndex(x => x.text == card.CardType.ToString());
            SubTypeDropdown.value = SubTypeDropdown.options.FindIndex(x => x.text == card.SubTypes.ToString());
            SymbolDropdown.value = SymbolDropdown.options.FindIndex(x => x.text == card.Symbol?.ToString());
            EffectTextField.text = card.EffectText;
            FlavourTextField.text = card.FlavourText;
            AttackDropdown.value = AttackDropdown.options.FindIndex(x => x.text == card.Attack.ToString());
            DefenceDropdown.value = DefenceDropdown.options.FindIndex(x => x.text == card.Defence.ToString());
            HasPersistenceToggle.isOn = card.HasPersistence;
            IsUniqueToggle.isOn = card.IsUnique;
        }
    }
}
