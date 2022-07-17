using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ManageDecksController : MonoBehaviour
    {
        // Resources
        private Button CardRow { get; set; }

        private Dropdown DeckDropdown { get; set; }
        private Dropdown CardDropown { get; set; }
        private Transform DeckContentTransform { get; set; }

        private List<CardData> Cards { get; set; }

        void Start()
        {
            // Load Resources
            CardRow = Resources.Load<Button>("Prefabs/CardRow");

            // Initialize
            DeckContentTransform = GameObject.Find("Canvas/DeckView/Viewport/DeckContent").transform;

            DeckDropdown = GameObject.Find("Canvas/DeckDropdown").GetComponent<Dropdown>();
            DeckDropdown.AddOptions(Enumerable.Repeat("Select Deck", 1).Concat(PlayerIO.ReadDeckNames()).ToList());
            DeckDropdown.onValueChanged.AddListener(DeckDropdownChanged);

            Cards = DataIO.ReadAll<CardData>().ToList();
            CardDropown = GameObject.Find("Canvas/CardDropdown").GetComponent<Dropdown>();
            CardDropown.AddOptions(Cards.Select(x => x.Name).ToList());
            GameObject.Find("Canvas/AddCardButton").GetComponent<Button>()
                .onClick.AddListener(AddNewCard);

            GameObject.Find("Canvas/SaveButton").GetComponent<Button>()
                .onClick.AddListener(SaveDeck);

            GameObject.Find("Canvas/AddDeckButton").GetComponent<Button>()
                .onClick.AddListener(CreateEmptyDeck);
        }

        private void DeckDropdownChanged(int index)
        {
            var deckName = DeckDropdown.captionText.text;
            var deckCards = PlayerIO.ReadDeck(deckName);

            // Clear current deck
            for (var i = 0; i < DeckContentTransform.childCount; i++) {
                Destroy(DeckContentTransform.GetChild(i).gameObject);
            }

            // Add new deck
            foreach (var card in deckCards) {
                AddCardRow(Cards.Single(x => x.Id == card));
            }
        }

        private void AddCardRow(CardData cardData)
        {
            var button = Instantiate(CardRow, Vector3.zero, Quaternion.identity);
            button.transform.Find("Text").GetComponent<Text>().text = cardData.Name;
            button.onClick.AddListener(() => { Destroy(button.gameObject); });
            button.transform.SetParent(DeckContentTransform);
        }

        private void AddNewCard()
        {
            var index = CardDropown.value;
            var card = Cards[index];
            AddCardRow(card);
        }

        private void SaveDeck()
        {
            var newDeck = new List<int>();
            for (var i = 0; i < DeckContentTransform.childCount; i++) {
                var cardName = DeckContentTransform.GetChild(i).Find("Text").GetComponent<Text>().text;
                var id = Cards.First(x => x.Name == cardName).Id;
                newDeck.Add(id);
            }

            PlayerIO.WriteDeck(DeckDropdown.captionText.text, newDeck.OrderBy(x => x));
        }

        private void CreateEmptyDeck()
        {
            var deckName = GameObject.Find("Canvas/NewDeckName").GetComponent<TMP_InputField>().text;

            DeckDropdown.AddOptions(new List<string>() { deckName });

            PlayerIO.WriteDeck(deckName, Enumerable.Empty<int>());
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
