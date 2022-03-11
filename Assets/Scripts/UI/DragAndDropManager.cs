using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class DragAndDropManager : MonoBehaviour
    {
        // Allowing reference to main controller here due to nature of Update()
        private MainController MainController { get; set; }

        private HashSet<CardObject> AvailableCards { get; set; }
        private HashSet<SlotObject> AvailableSlots { get; set; }
        private Func<CardObject, SlotObject, IEnumerable<BaseEvent>> OnDrop { get; set; }
        private Func<IEnumerable<BaseEvent>> OnPass { get; set; }

        private CardObject SelectedCard { get; set; }
        private Vector2 OriginalPosition { get; set; }

        public bool IsProcessing { get; set; }

        public void Begin(HashSet<CardObject> availableCards, HashSet<SlotObject> availableSlots, Func<CardObject, SlotObject, IEnumerable<BaseEvent>> onDrop, Func<IEnumerable<BaseEvent>> onPass)
        {
            MainController = MainController.Get();
            AvailableCards = availableCards;
            AvailableSlots = availableSlots;
            OnDrop = onDrop;
            OnPass = onPass;
            IsProcessing = true;
        }

        public void Finish(SlotObject slot)
        {
            var newEvents = OnDrop.Invoke(SelectedCard, slot).ToArray();

            // If no new events, either unable to play cards or discarded drop
            if (!newEvents.Any())
            {
                // Not enough mana message
                DropCard();
            }
            else
            {
                DropInSlot(slot);
                IsProcessing = false;
            }

            MainController.EnqueueEvents(newEvents);
        }

        public void Pass()
        {
            MainController.EnqueueEvents(OnPass.Invoke());

            SelectedCard = null;
            IsProcessing = false;
        }

        private void DropCard()
        {
            SelectedCard.MoveToPosition(OriginalPosition);
            SelectedCard.SetSortingOrder(0);
            SelectedCard = null;
        }

        private void DropInSlot(SlotObject slot)
        {
            // Drop non-action cards
            if (SelectedCard.CardReference.Type != CardType.Action)
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
