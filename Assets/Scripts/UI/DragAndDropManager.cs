using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class DragAndDropManager : MonoBehaviour
    {
        private HashSet<CardObject> AvailableCards { get; set; }
        private HashSet<SlotObject> AvailableSlots { get; set; }
        private Func<CardObject, SlotObject, bool> OnDroppedInSlot { get; set; }
        private Action OnPassed { get; set; }

        private CardObject SelectedCard { get; set; } // card currently being held
        private Vector2 OriginalPosition { get; set; }

        public bool IsProcessing { get; set; }

        public void Begin(
            IEnumerable<CardObject> availableCards,
            IEnumerable<SlotObject> availableSlots,
            Func<CardObject, SlotObject, bool> onDroppedInSlot,
            Action onPassed)
        {
            AvailableCards = new HashSet<CardObject>(availableCards);
            AvailableSlots = new HashSet<SlotObject>(availableSlots);
            OnDroppedInSlot = onDroppedInSlot;
            OnPassed = onPassed;
            IsProcessing = true;
        }

        private void Finish(SlotObject slot)
        {
            var succesfullyDropped = OnDroppedInSlot(SelectedCard, slot);

            if (succesfullyDropped) 
            {
                DropInSlot(slot);
            }
            else 
            {
                DropCard();
            }

            IsProcessing = false;
        }

        private void Pass()
        {
            OnPassed();

            SelectedCard = null;
            IsProcessing = false;
        }

        private void DropCard()
        {
            SelectedCard.MoveToPosition(OriginalPosition);
            SelectedCard.SetSortingOrder(0);
            SelectedCard = null;
        }

        private void DropInSlot(SlotObject slot, bool dropActionCards = false)
        {
            // Only drop non-action cards and action cards if dropActionCard is false
            if (dropActionCards || SelectedCard.CardReference.Type != CardType.Action)
            {
                SelectedCard.MoveToPosition(slot.transform.position);
                SelectedCard.SetSortingOrder(0);
            }

            SelectedCard = null;
        }

        // Replace with OnMouseDrag() ?
        private void Update()
        {
            if (!IsProcessing)
                return;

            // On click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Card"));

                if (hit.collider != null)
                {
                    var selectedCard = hit.collider.GetComponentInParent<CardObject>();

                    if (AvailableCards.Contains(selectedCard))
                    {
                        SelectedCard = selectedCard;
                        OriginalPosition = selectedCard.transform.position;
                        selectedCard.SetSortingOrder(1);
                    }
                }
            }

            // Holding
            if (Input.GetMouseButton(0))
            {
                if (SelectedCard != null)
                {
                    SelectedCard.SetTargetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }

            // Dropping
            if (Input.GetMouseButtonUp(0))
            {
                if (SelectedCard != null)
                {
                    // Get all touching slots
                    var cardCollider = SelectedCard.GetComponent<Collider2D>();
                    var contactFilter = new ContactFilter2D();
                    contactFilter.SetLayerMask(LayerMask.GetMask("Slot"));
                    var touchingSlots = new List<Collider2D>();

                    cardCollider.OverlapCollider(contactFilter, touchingSlots);
                    var validSlots = touchingSlots.Select(x => x.GetComponent<SlotObject>());

                    // If not an action card only allow empty slots
                    if (SelectedCard.CardReference.Type != CardType.Action)
                    {
                        validSlots = validSlots.Intersect(AvailableSlots);
                    }

                    var closestSlot = validSlots.OrderBy(x => Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), x.transform.position)).FirstOrDefault();

                    if (closestSlot == null)
                    {
                        DropCard();
                        return;
                    }

                    Finish(closestSlot);
                }
            }

            // Passing
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Pass();
            }
        }
    }
}
