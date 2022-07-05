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
        // Allowing reference to main controller here due to nature of Update()
        private MainController MainController { get; set; }

        private HashSet<CardObject> AvailableCards { get; set; }
        private HashSet<SlotObject> AvailableSlots { get; set; }
        private Func<BaseCard, FieldSlot, IEnumerable<BaseEvent>> OnDrop { get; set; }
        private Func<BaseCard, IEnumerable<BaseEvent>> OnSacrifice { get; set; }
        private Func<IEnumerable<BaseEvent>> OnPass { get; set; }

        private CardObject SelectedCard { get; set; }
        private Vector2 OriginalPosition { get; set; }

        public bool IsProcessing { get; set; }

        public void Begin(IEnumerable<CardObject> availableCards, IEnumerable<SlotObject> availableSlots, Func<BaseCard, FieldSlot, IEnumerable<BaseEvent>> onDrop, Func<BaseCard, IEnumerable<BaseEvent>> onSacrifice, Func<IEnumerable<BaseEvent>> onPass)
        {
            MainController = MainController.Get();
            AvailableCards = new HashSet<CardObject>(availableCards);
            AvailableSlots = new HashSet<SlotObject>(availableSlots);
            OnDrop = onDrop;
            OnSacrifice = onSacrifice;
            OnPass = onPass;
            IsProcessing = true;
        }

        public void Finish(SlotObject slot)
        {
            var newEvents = new List<BaseEvent>();

            if (slot.SlotType == SlotType.Mana) {
                newEvents.AddRange(OnSacrifice.Invoke(SelectedCard.CardReference));
                DropInSlot(slot, true);
            }
            else
            {
                newEvents.AddRange(OnDrop.Invoke(SelectedCard.CardReference, slot.SlotReference));
                // If no new events, unable to pay mana cost
                if (!newEvents.Any()) {
                    // Not enough mana message
                    DropCard();
                    return;
                }
                DropInSlot(slot);
            }

            MainController.EnqueueEvents(newEvents);
            IsProcessing = false;
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

        private void DropInSlot(SlotObject slot, bool dropActionCards = false)
        {
            // Only drop non-action cards if dropActionCard is false
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
