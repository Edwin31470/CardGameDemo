using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SelectionManager : MonoBehaviour
    {
        // Allowing reference to main controller here due to nature of Update()
        private MainController MainController { get; set; }

        // Card Selection
        private HashSet<CardObject> SelectedCards { get; set; }
        private HashSet<CardObject> ValidCards { get; set; }
        private int MaxTargets { get; set; }
        private Color SelectionColour { get; set; }

        private OnFinishSelection OnFinishSelection { get; set; }

        public bool IsProcessing { get; set; }

        public void Start()
        {
            MainController = MainController.Get();
            SelectedCards = new HashSet<CardObject>();
            ValidCards = new HashSet<CardObject>();
        }

        public void Begin(HashSet<CardObject> validCards, int maxTargets, OnFinishSelection onFinishSelection, SelectionType selectionType)
        {
            IsProcessing = true;
            ValidCards = validCards;
            MaxTargets = maxTargets;
            OnFinishSelection = onFinishSelection;

            switch (selectionType)
            {
                case SelectionType.Neutral:
                    SelectionColour = new Color(0.9f, 0.7f, 0.15f);
                    break;
                case SelectionType.Positive:
                    SelectionColour = new Color(0.12f, 0.62f, 0.16f);
                    break;
                case SelectionType.Negative:
                    SelectionColour = new Color(0.86f, 0.27f, 0.12f);
                    break;
            }
        }

        /// <summary>
        /// Finish selection and return events of that selection
        /// </summary>
        /// <returns>Events resulting from selection</returns>
        public void Finish()
        {
            var selectedCards = SelectedCards.Select(x => x.CardReference);
            MainController.EnqueueEvents(OnFinishSelection.Invoke(selectedCards));

            DeselectCards();
            IsProcessing = false;
        }

        private void DeselectCards()
        {
            foreach (var card in SelectedCards.ToList())
            {
                card.Dehighlight();
                SelectedCards.Remove(card);
            }
        }

        private void Update()
        {
            if (!IsProcessing)
                return;


            // Get Colliding Card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Card"));

            // Null when not hovering
            var hoveredCard = hit.collider?.GetComponentInParent<CardObject>();

            // Clicking on a card
            if (Input.GetMouseButtonDown(0))
            {
                if (ValidCards.Contains(hoveredCard))
                {
                    // Select Card
                    if (!SelectedCards.Contains(hoveredCard))
                    {
                        SelectedCards.Add(hoveredCard);
                    }
                    // Deselect Card
                    else
                    {
                        SelectedCards.Remove(hoveredCard);
                    }
                }
            }


            // Dehighlight When Not Selected
            foreach (var card in ValidCards.Where(x => !SelectedCards.Contains(x) && x != hoveredCard))
            {
                card.Dehighlight();
            }

            // Highlight When Hovering
            if (ValidCards.Contains(hoveredCard))
            {
                hoveredCard.Highlight(SelectionColour);
            }


            // Finish if all targets selected or no more targets
            if (ShouldFinishSelection())
            {
                Finish();
            }
        }

        private bool ShouldFinishSelection()
        {
            if (SelectedCards.Count == MaxTargets) {
                return true;
            }
            if (SelectedCards.Count == ValidCards.Count) {
                return true;
            }
            // TODO: Only some events can have less than maximum targets selected
            if (Input.GetKeyDown(KeyCode.Return)) {
                return true;
            }

            return false;
        }
    }
}
