using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CreateCardController : MonoBehaviour
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
        private Toggle HasPersistence { get; set; }
        private Toggle IsUnique { get; set; }


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

            HasPersistence = GameObject.Find("Canvas/HasPersistenceToggle").GetComponent<Toggle>();
            IsUnique = GameObject.Find("Canvas/IsUniqueToggle").GetComponent<Toggle>();

            // Register button
            GameObject.Find("Canvas/CreateButton").GetComponent<Button>()
                .onClick.AddListener(CreateCard);
        }

        private void CreateCard()
        {
            var id = CardIO.ReadAll().Max(x => x.Id) + 1;

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

            var hasPersistence = HasPersistence.isOn;
            var isUnique = IsUnique.isOn;

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
            return new List<string>
            {
                "Elemental",
                "Skull"
            };
        }
    }
}
