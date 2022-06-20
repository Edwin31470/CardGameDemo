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
        public bool IsProcessing => SelectionManager.IsProcessing || DragAndDropManager.IsProcessing;

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

        // Keep track of cards and slots
        private HashSet<CardObject> CardObjects { get; set; }
        private List<SlotObject> FieldSlotObjects { get; set; }
        private HashSet<SlotObject> ManaSlotObjects { get; set; }
        private IEnumerable<SlotObject> SlotObjects => FieldSlotObjects.Concat(ManaSlotObjects);

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

        public void Initialize(BoardState board)
        {
            // Initialize collections
            CardObjects = new HashSet<CardObject>();
            FieldSlotObjects = new List<SlotObject>();
            ManaSlotObjects = new HashSet<SlotObject>();

            foreach (var player in board.BothPlayers)
            {
                var fieldSlotObjects = FindObjectsOfType<SlotObject>()
                    .Where(x => x.Owner == player.PlayerType && x.SlotType == SlotType.Field)
                    .ToArray();

                foreach (var slotObject in fieldSlotObjects)
                {
                    slotObject.FieldSlot = player.Field[slotObject.Index];
                }

                FieldSlotObjects.AddRange(fieldSlotObjects);
            }
        }

        private CardObject CreateCard(PlayerType playerType, BaseCard card)
        {
            var newCard = Instantiate(CardObject, playerType == PlayerType.Front ? FrontDeckOrigin : BackDeckOrigin, Quaternion.identity);
            newCard.Initialize(card);
            newCard.SetSortingLayer("Card");

            CardObjects.Add(newCard);
            return newCard;
        }

        private IEnumerable<SlotObject> GetSlotObjects(PlayerType playerType, IEnumerable<FieldSlot> fieldSlots)
        {
            return SlotObjects.Where(x => fieldSlots.Contains(x.FieldSlot)).Concat(ManaSlotObjects.Where(x => x.Owner == playerType));
        }

        private IEnumerable<SlotObject> GetFieldSlotObjects(IEnumerable<FieldSlot> fieldSlots)
        {
            return FieldSlotObjects.Where(x => fieldSlots.Contains(x.FieldSlot));
        }

        private SlotObject GetFieldSlotObject(FieldSlot slot)
        {
            return FieldSlotObjects.Single(x => x.FieldSlot == slot);
        }

        private IEnumerable<CardObject> GetCardObjects(IEnumerable<BaseCard> cardReferences)
        {
            return CardObjects.Where(x => cardReferences.Contains(x.CardReference));
        }

        private CardObject GetCardObject(BaseCard card)
        {
            return CardObjects.Single(x => x.CardReference == card);
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
                    throw new ArgumentOutOfRangeException(nameof(player), $"Only {PlayerType.Front} or {PlayerType.Back} are allowed");
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

        public void BeginCardSelection(
            IEnumerable<BaseCard> allowableTargets,
            IEnumerable<BaseCard> overrideTargets,
            int count,
            Func<IEnumerable<BaseCard>, IEnumerable<BaseEvent>> onFinishSelection,
            SelectionType selectionType)
        {
            // TODO: null or empty?
            var targets = overrideTargets != null ?
                GetCardObjects(overrideTargets) :
                GetCardObjects(allowableTargets);

            SelectionManager.Begin(
                targets,
                count,
                onFinishSelection,
                selectionType);
        }

        public void BeginDragAndDrop(IEnumerable<BaseCard> draggableCards, IEnumerable<FieldSlot> availableFieldSlots, PlayerType playingPlayer, Func<BaseCard, FieldSlot, IEnumerable<BaseEvent>> onDrop, Func<BaseCard, IEnumerable<BaseEvent>> onSacrifice, Func<IEnumerable<BaseEvent>> onPass)
        {
            DragAndDropManager.Begin(
                GetCardObjects(draggableCards),
                GetSlotObjects(playingPlayer, availableFieldSlots),
                onDrop,
                onSacrifice,
                onPass);
        }

        public void CreateInHand(PlayerType playerType, BaseCard card)
        {
            CreateCard(playerType, card);
        }

        public void CreateInSlot(PlayerType playerType, BaseCard card, FieldSlot slot)
        {
            var cardObject = CreateCard(playerType, card);
            var slotObject = GetFieldSlotObject(slot);
            cardObject.SetTargetPosition(slotObject.transform.position);
        }

        public void MoveToSlot(FieldCard card, FieldSlot slot)
        {
            var cardObject = GetCardObject(card);
            var slotObject = GetFieldSlotObject(slot);
            cardObject.SetTargetPosition(slotObject.transform.position);
        }

        public void UpdateHand(PlayerType playerType, IEnumerable<BaseCard> handCards)
        {
            var freeCards = GetCardObjects(handCards).ToList();

            var handPoints = GetHandPoints(playerType, freeCards.Count);

            foreach (var point in handPoints)
            {
                var closestCard = freeCards.OrderBy(x => Vector2.Distance(x.transform.position, point)).First();
                closestCard.SetTargetPosition(new Vector2(point.x, point.y));
                freeCards.Remove(closestCard);
            }
        }

        public void ReturnToDeck(PlayerType playerType, BaseCard card)
        {
            var cardObject = GetCardObject(card);
            cardObject.SetTargetPosition(playerType == PlayerType.Front ? FrontDeckOrigin : BackDeckOrigin);
            cardObject.DestroyWhenInPosition = true;
        }

        public void DestroyCard(PlayerType playerType, BaseCard card)
        {
            var cardObject = GetCardObject(card);
            cardObject.SetTargetPosition(playerType == PlayerType.Front ? FrontDestroyLocation : BackDestroyLocation);
            cardObject.DestroyWhenInPosition = true;

            CardObjects.Remove(cardObject);
        }

        public void SacrificeCard(BaseCard card)
        {
            var cardObject = GetCardObject(card);
            cardObject.SetTargetPosition(cardObject.transform.position);
            cardObject.DestroyWhenInPosition = true;

            CardObjects.Remove(cardObject);

        }

        public void UpdateSlotGlow(FieldSlot slot, EffectType type)
        {
            var slotObject = GetFieldSlotObject(slot);
            slotObject.SetGlow(type);
        }
    }
}
