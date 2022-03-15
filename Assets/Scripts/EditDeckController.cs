using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EditDeckController : MonoBehaviour
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
            DeckDropdown.AddOptions(Enumerable.Repeat("Select Deck", 1).Concat(DeckIO.ReadDeckNames()).ToList());
            DeckDropdown.onValueChanged.AddListener(DeckDropdownChanged);

            Cards = CardIO.ReadAll().ToList();
            CardDropown = GameObject.Find("Canvas/CardDropdown").GetComponent<Dropdown>();
            CardDropown.AddOptions(Cards.Select(x => x.Name).ToList());
            GameObject.Find("Canvas/AddCardButton").GetComponent<Button>()
                .onClick.AddListener(AddNewCard);

            GameObject.Find("Canvas/SaveButton").GetComponent<Button>()
                .onClick.AddListener(SaveDeck);
        }

        private void DeckDropdownChanged(int index)
        {
            var deckName = DeckDropdown.captionText.text;
            var deckCards = DeckIO.ReadDeck(deckName);

            // Clear current deck
            for (var i = 0; i < DeckContentTransform.childCount; i++) {
                Destroy(DeckContentTransform.GetChild(i).gameObject);
            }

            // Add new deck
            foreach (var card in deckCards) {
                AddCardRow(Cards[card]);
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

            DeckIO.WriteDeck(DeckDropdown.captionText.text, newDeck.OrderBy(x => x));
        }
    }
}
