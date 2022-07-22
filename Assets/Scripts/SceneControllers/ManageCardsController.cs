using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.IO;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManageCardsController : MonoBehaviour
    {
        // Resources
        private GameObject CardCanvasObject { get; set; }

        // Managers
        private CanvasHoverManager HoverManager { get; set; }


        // Fields for editing/creating
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

        // Fields for UI
        private Transform AllCardsTransform { get; set; }
        private CardData CurrentlySelected { get; set; }

        public void Start()
        {
            // Load Resources
            CardCanvasObject = Resources.Load<GameObject>("Prefabs/CardCanvasObject");

            // Create Managers
            HoverManager = gameObject.AddComponent<CanvasHoverManager>();

            // Register Card Properties
            NameField = GameObject.Find("Canvas/NameField").GetComponent<InputField>();

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

            EffectTextField = GameObject.Find("Canvas/EffectTextField").GetComponent<InputField>();
            FlavourTextField = GameObject.Find("Canvas/FlavourTextField").GetComponent<InputField>();

            HasPersistenceToggle = GameObject.Find("Canvas/HasPersistenceToggle").GetComponent<Toggle>();
            IsUniqueToggle = GameObject.Find("Canvas/IsUniqueToggle").GetComponent<Toggle>();

            // Register All Card list
            AllCardsTransform = GameObject.Find("Canvas/CardsView/Viewport/Content").transform;
            PopulateAllCardsScrollView();

            // Register button
            GameObject.Find("Canvas/SubmitButton").GetComponent<Button>()
                .onClick.AddListener(Submit);
        }

        private void PopulateAllCardsScrollView()
        {
            // Clear existing
            for (var i = 0; i < AllCardsTransform.childCount; i++)
            {
                Destroy(AllCardsTransform.GetChild(i).gameObject);
            }

            // Fill new
            var cards = DataIO.ReadAll<CardData>()
                .OrderBy(x => x.Colour)
                .ThenBy(x => x.CardType)
                .ThenBy(x => x.Cost)
                .ThenBy(x => x.Name);

            foreach (var card in cards)
            {
                var cardCanvasObject = Instantiate(CardCanvasObject, Vector2.zero, Quaternion.identity, AllCardsTransform);
                cardCanvasObject.AddComponent<Metadata>().AddData("id", card.Id);

                cardCanvasObject.GetComponent<Image>().sprite = SpritesManager.GetSmallCard(card.Colour, card.CardType);
                cardCanvasObject.transform.Find("Cost").GetComponent<Image>().sprite = SpritesManager.GetCostSymbol(card.Cost);
                cardCanvasObject.transform.Find("Symbol").GetComponent<Image>().sprite = SpritesManager.GetCardSymbol(card.Symbol);

                if (card.CardType == CardType.Creature)
                {
                    cardCanvasObject.transform.Find("Attack").GetComponent<TMP_Text>().text = card.Attack.ToString();
                    cardCanvasObject.transform.Find("Defence").GetComponent<TMP_Text>().text = card.Defence.ToString();
                }
                else
                {
                    cardCanvasObject.transform.Find("Attack").GetComponent<TMP_Text>().text = "";
                    cardCanvasObject.transform.Find("Defence").GetComponent<TMP_Text>().text = "";
                }
            }
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

        private void Submit()
        {
            // Retrieve or create id
            int id;
            if (CurrentlySelected == null)
            {
                var existingCards = DataIO.ReadAll<CardData>();
                id = existingCards.Max(x => x.Id) + 1;
            }
            else
            {
                id = CurrentlySelected.Id;
            }

            // Retrieve data from the UI
            var data = new CardData
            {
                Id = id,
                Name = NameField.text,

                Colour = (Colour)Enum.Parse(typeof(Colour), ColourDropdown.captionText.text),
                Cost = int.Parse(CostDropdown.captionText.text),

                CardType = (CardType)Enum.Parse(typeof(CardType), CardTypeDropdown.captionText.text),
                SubTypes = (SubType)Enum.Parse(typeof(SubType), SubTypeDropdown.captionText.text),

                Symbol = SymbolDropdown.captionText.text,

                Attack = int.Parse(AttackDropdown.captionText.text),
                Defence = int.Parse(DefenceDropdown.captionText.text),

                EffectText = EffectTextField.text,
                FlavourText = FlavourTextField.text,

                HasPersistence = HasPersistenceToggle.isOn,
                IsUnique = IsUniqueToggle.isOn
            };

            // Write
            DataIO.Write(data);

            // Clear and prepare UI
            PopulateComponents(new CardData());
            PopulateAllCardsScrollView();
        }

        private void PopulateComponents(CardData card)
        {
            NameField.text = card.Name;

            ColourDropdown.value = ColourDropdown.options.FindIndex(x => x.text == card.Colour.ToString());
            CostDropdown.value = CostDropdown.options.FindIndex(x => x.text == card.Cost.ToString());

            CardTypeDropdown.value = CardTypeDropdown.options.FindIndex(x => x.text == card.CardType.ToString());
            SubTypeDropdown.value = SubTypeDropdown.options.FindIndex(x => x.text == card.SubTypes.ToString());

            SymbolDropdown.value = SymbolDropdown.options.FindIndex(x => x.text == card.Symbol?.ToString());

            AttackDropdown.value = AttackDropdown.options.FindIndex(x => x.text == card.Attack.ToString());
            DefenceDropdown.value = DefenceDropdown.options.FindIndex(x => x.text == card.Defence.ToString());

            EffectTextField.text = card.EffectText;
            FlavourTextField.text = card.FlavourText;

            HasPersistenceToggle.isOn = card.HasPersistence;
            IsUniqueToggle.isOn = card.IsUnique;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("StartScreen");
            }

            var (hoveredCard, cardData) = EventSystem.current.GetHoveredCardAndData();

            if (hoveredCard == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                // Deselect a currently selected card
                if (CurrentlySelected?.Id == cardData.Id)
                {
                    CurrentlySelected = null;
                    PopulateComponents(new CardData());
                }
                // Select hovered card
                else
                {
                    CurrentlySelected = cardData;
                    PopulateComponents(cardData);
                }
            }
        }
    }
}
