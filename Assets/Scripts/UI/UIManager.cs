using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        private static CardObject CardObject { get; set; }
        private SelectionManager SelectionManager { get; set; }
        private DragAndDropManager DragAndDropManager { get; set; }

        // Locations on screen
        private static Vector2 FrontDeckOrigin { get; set; }
        private static Vector2 BackDeckOrigin { get; set; }
        private static Vector2 FrontDestroyLocation { get; set; }
        private static Vector2 BackDestroyLocation { get; set; }

        // Turn Light Sprites
        private SpriteRenderer FrontTurnLight { get; set; }
        private SpriteRenderer BackTurnLight { get; set; }

        public bool IsProcessing => SelectionManager.IsProcessing || DragAndDropManager.IsProcessing;

        public void Start()
        {
            // Load Resources
            CardObject = Resources.Load<CardObject>("Prefabs/CardObject");

            // Setup Managers
            SelectionManager = gameObject.AddComponent(typeof(SelectionManager)) as SelectionManager;
            DragAndDropManager = gameObject.AddComponent(typeof(DragAndDropManager)) as DragAndDropManager;

            // Get Locations
            FrontDeckOrigin = GameObject.Find("FrontPlayer/DeckOrigin").transform.position;
            BackDeckOrigin = GameObject.Find("BackPlayer/DeckOrigin").transform.position;
            FrontDestroyLocation = GameObject.Find("FrontPlayer/DestroyedLocation").transform.position;
            BackDestroyLocation = GameObject.Find("BackPlayer/DestroyedLocation").transform.position;

            // Get Turn Light Sprites
            FrontTurnLight = GameObject.Find("FrontPlayer/ActiveTurnLight").GetComponent<SpriteRenderer>();
            BackTurnLight = GameObject.Find("BackPlayer/ActiveTurnLight").GetComponent<SpriteRenderer>();
        }

        public void RegisterSlots(Player player)
        {
            var slots = GetAvailableSlots(player.PlayerType).Where(x => x.SlotType == SlotType.Field);
            foreach (var slot in slots)
            {
                slot.FieldSlot = player.GetField()[slot.Index];
            }
        }

        private CardObject CreateCard(BaseCard card)
        {
            var newCard = Instantiate(CardObject, card.Owner.PlayerType == PlayerType.Front ? FrontDeckOrigin : BackDeckOrigin, Quaternion.identity);
            newCard.Initialize(card);
            newCard.SetSortingLayer("Card");
            return newCard;
        }

        private HashSet<SlotObject> GetAvailableSlots(PlayerType playerType)
        {
            // TODO: UIManager should keep track of slot references
            var slots = FindObjectsOfType<SlotObject>()
                .Where(x => x.Owner == playerType && !x.IsOccupied());

            return new HashSet<SlotObject>(slots);
        }

        private HashSet<CardObject> GetAvailableCards(TargetConditions targetConditions)
        {
            // TODO: UIManager should keep track of card references
            var cardObjects = FindObjectsOfType<CardObject>().Where(x => targetConditions.IsMatch(x.CardReference));

            return new HashSet<CardObject>(cardObjects);
        }

        public void UpdateHand(PlayerType player)
        {
            var freeCards = GetAvailableCards(new TargetConditions 
            {
                PlayerType = player,
                Area = Area.Hand
            });

            var handPoints = GetHandPoints(player, freeCards.Count());

            foreach (var point in handPoints)
            {
                var closestCard = freeCards.OrderBy(x => Vector2.Distance(x.transform.position, point)).First();
                closestCard.SetTargetPosition(new Vector2(point.x, point.y));
                freeCards.Remove(closestCard);
            }
        }

        private IEnumerable<Vector2> GetHandPoints(PlayerType player, int numberOfPoints)
        {
            // Get hand box
            string objectName;
            switch (player)
            {
                case PlayerType.Front:
                    objectName = "FrontPlayer/Hand";
                    break;
                case PlayerType.Back:
                    objectName = "BackPlayer/Hand";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("player", $"Only {PlayerType.Front} or {PlayerType.Back} are allowed");
            }

            BoxCollider2D collider = GameObject.Find(objectName).GetComponent<BoxCollider2D>();

            // Calculate equidistant points
            var width = collider.bounds.size.x;
            var farLeft = collider.bounds.center.x - (width / 2);
            var y = collider.bounds.center.y;

            var points = new List<Vector2>();

            if (numberOfPoints <= 7)
            {
                var segmentLength = width / 7; // distance between points
                for (int i = 0; i < numberOfPoints; i++)
                {
                    var x = (farLeft + segmentLength / 2) + (i * segmentLength);
                    points.Add(new Vector2(x, y));
                }
            }
            else
            {
                var segmentLength = width / (numberOfPoints + 1); // distance between points
                for (int i = 0; i < numberOfPoints; i++)
                {
                    var x = (farLeft + segmentLength) + (i * segmentLength);
                    points.Add(new Vector2(x, y));
                }
            }


            return points;
        }

        public void SetTurn(PlayerType playerType)
        {
            FrontTurnLight.enabled = playerType == PlayerType.Front;
            BackTurnLight.enabled = playerType == PlayerType.Back;
        }

        public void BeginCardSelection(TargetConditions targetConditions, IEnumerable<BaseCard> overrideTargets, int count, OnFinishSelection onFinishSelection, SelectionType selectionType)
        {
            // TODO: null or empty?
            var targets = overrideTargets != null ?
                new HashSet<CardObject>(GetCardObjects(overrideTargets)) :
                GetAvailableCards(targetConditions);

            SelectionManager.Begin(
                targets,
                count,
                onFinishSelection,
                selectionType);
        }

        public void BeginDragAndDrop(TargetConditions targetConditions, Func<CardObject, SlotObject, IEnumerable<BaseEvent>> onDrop, Func<IEnumerable<BaseEvent>> onPass)
        {
            DragAndDropManager.Begin(
                GetAvailableCards(targetConditions),
                GetAvailableSlots(targetConditions.PlayerType),
                onDrop,
                onPass);
        }

        public void CreateInHand(BaseCard card)
        {
            CreateCard(card);
            UpdateHand(card.Owner.PlayerType);
        }

        public void CreateInRandomSlot(BaseCard card)
        {
            var cardObject = CreateCard(card);
            var slot = GetAvailableSlots(card.Owner.PlayerType).Where(x => x.SlotType != SlotType.Mana).OrderBy(x => Guid.NewGuid()).First();
            cardObject.SetTargetPosition(slot.transform.position);
        }

        public void MoveToRandomSlot(BaseCard card)
        {
            var cardObject = GetCardObject(card);
            var slot = GetAvailableSlots(card.Owner.PlayerType).Where(x => x.SlotType != SlotType.Mana).OrderBy(x => Guid.NewGuid()).First();
            cardObject.SetTargetPosition(slot.transform.position);
        }

        public void ReturnToDeck(CardObject cardObject)
        {
            cardObject.SetTargetPosition(cardObject.CardReference.Owner.PlayerType == PlayerType.Front ? FrontDeckOrigin : BackDeckOrigin);
            cardObject.DestroyWhenInPosition = true;
        }

        public void Destroy(CardObject cardObject)
        {
            cardObject.SetTargetPosition(cardObject.CardReference.Owner.PlayerType == PlayerType.Front ? FrontDestroyLocation : BackDestroyLocation);
            cardObject.DestroyWhenInPosition = true;
        }

        public CardObject GetCardObject(BaseCard card)
        {
            var targetConditions = new TargetConditions {PlayerType = card.Owner.PlayerType, Area = Area.Any};

            var cardObject = GetAvailableCards(targetConditions).FirstOrDefault(x => x.CardReference == card);

            return cardObject;
        }

        public IEnumerable<CardObject> GetCardObjects(IEnumerable<BaseCard> cards)
        {
            var cardObjects = GetAvailableCards(new TargetConditions { Area = Area.Any })
                .Where(x => cards.Contains(x.CardReference));

            return cardObjects;
        }
    }
}
