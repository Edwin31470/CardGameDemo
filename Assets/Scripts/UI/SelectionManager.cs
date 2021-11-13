using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SelectionManager : MonoBehaviour
    {
        // Card Selection
        private HashSet<CardObject> SelectedCards { get; set; }
        private HashSet<CardObject> ValidCards { get; set; }
        private int MaxTargets { get; set; }
        private Color SelectionColour { get; set; }

        private Action<IEnumerable<BaseCard>> Action { get; set; }

        public bool IsProcessing { get; set; }

        public void Start()
        {
            SelectedCards = new HashSet<CardObject>();
        }

        public void Begin(HashSet<CardObject> validCards, int maxTargets, Action<IEnumerable<BaseCard>> action, SelectionType selectionType)
        {
            IsProcessing = true;
            ValidCards = validCards;
            MaxTargets = maxTargets;
            Action = action;

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

        public void Finish()
        {
            Action.Invoke(SelectedCards.Select(x => x.CardReference));
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
            if (SelectedCards.Count() == MaxTargets || SelectedCards.Count() == ValidCards.Count())
            {
                Finish();
            }

            // Only some events can have less than maximum targets selected
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Finish();
            }
        }
    }
}
