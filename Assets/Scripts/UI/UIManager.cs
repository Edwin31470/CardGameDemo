using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Events;
using UnityEngine;
using Assets.Scripts.Bases;
using Assets.Scripts.Terrains;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI
{
    // Manages the interaction between the game logic and the UI
    public class UIManager : MonoBehaviour
    {
        private static CardObject CardObjectPrefab { get; set; }
        private static TerrainObject TerrainObjectPrefab { get; set; }
        private TargetManager TargetManager { get; set; }
        private DragAndDropManager DragAndDropManager { get; set; }
        private PileManager PileManager { get; set; }
        private ShowTokensManager ShowTokensManager { get; set; }
        private HoverManager HoverManager { get; set; }

        // Locations on screen
        private static Vector2 FrontDeckOrigin { get; set; }
        private static Vector2 BackDeckOrigin { get; set; }
        private Vector2 GetDeckOrigin(PlayerType playerType) => playerType == PlayerType.Front ? FrontDeckOrigin : BackDeckOrigin;

        private static Vector2 FrontDestroyLocation { get; set; }
        private static Vector2 BackDestroyLocation { get; set; }
        private Vector2 GetDestroyedOrigin(PlayerType playerType) => playerType == PlayerType.Front ? FrontDestroyLocation : BackDestroyLocation;

        // Turn Light Sprites
        private SpriteRenderer FrontTurnLight { get; set; }
        private SpriteRenderer BackTurnLight { get; set; }


        // Keep track of cards and slots
        private HashSet<CardObject> CardObjects { get; set; } = new();
        private List<SlotObject> FieldSlotObjects { get; set; } = new();
        private HashSet<SlotObject> ManaSlotObjects { get; set; } = new();
        private HashSet<TerrainObject> TerrainObjects { get; set; } = new();
        private IEnumerable<SlotObject> SlotObjects => FieldSlotObjects.Concat(ManaSlotObjects);

        // MainController method to add events to the queue
        private Action<IEnumerable<BaseEvent>> EnqueueEvents { get; set; }

        public bool IsProcessing => TargetManager.IsProcessing || DragAndDropManager.IsProcessing;

        public void Start()
        {
            // Load Resources
            CardObjectPrefab = Resources.Load<CardObject>("Prefabs/CardObject");
            TerrainObjectPrefab = Resources.Load<TerrainObject>("Prefabs/TerrainObject");

            // Setup Managers
            TargetManager = gameObject.AddComponent(typeof(TargetManager)) as TargetManager;
            TargetManager.Initialize(() => PileManager.IsShowing);

            DragAndDropManager = gameObject.AddComponent(typeof(DragAndDropManager)) as DragAndDropManager;
            DragAndDropManager.Initialize(() => PileManager.IsShowing);

            HoverManager = gameObject.AddComponent(typeof(HoverManager)) as HoverManager;
            HoverManager.Initialize(() => PileManager.IsShowing);

            PileManager = gameObject.AddComponent(typeof(PileManager)) as PileManager;

            ShowTokensManager = gameObject.AddComponent(typeof(ShowTokensManager)) as ShowTokensManager;


            // Get Locations
            FrontDeckOrigin = GameObject.Find("FrontPlayer/DeckOrigin").transform.position;
            BackDeckOrigin = GameObject.Find("BackPlayer/DeckOrigin").transform.position;
            FrontDestroyLocation = GameObject.Find("FrontPlayer/DestroyedLocation").transform.position;
            BackDestroyLocation = GameObject.Find("BackPlayer/DestroyedLocation").transform.position;

            // Get Turn Light Sprites
            FrontTurnLight = GameObject.Find("FrontPlayer/ActiveTurnLight").GetComponent<SpriteRenderer>();
            BackTurnLight = GameObject.Find("BackPlayer/ActiveTurnLight").GetComponent<SpriteRenderer>();
        }

        private Vector2 GetPileOrigin(Area area, PlayerType playerType)
        {
            switch (area)
            {
                case Area.Deck:
                    return GetDeckOrigin(playerType);
                case Area.Destroyed:
                    return GetDestroyedOrigin(playerType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(area), $"Must be {nameof(Area.Deck)} or {nameof(Area.Destroyed)}");
            }
        }

        public void Initialize(BoardState board, Action<IEnumerable<BaseEvent>> enqueueEvents)
        {
            EnqueueEvents = enqueueEvents;

            // Find and register slot objects - TODO: Generate and place the slot objects here instead
            foreach (var player in board.BothPlayers)
            {
                var slotObjects = FindObjectsOfType<SlotObject>().Where(x => x.Owner == player.PlayerType);

                var fieldSlotObjects = slotObjects.Where(x => x.SlotType == SlotType.Field)
                    .ToArray();

                foreach (var slotObject in fieldSlotObjects)
                {
                    slotObject.SlotReference = player.Field[slotObject.Index];
                }

                FieldSlotObjects.AddRange(fieldSlotObjects);
                ManaSlotObjects.Add(slotObjects.Single(x => x.SlotType == SlotType.Mana));
            }
        }

        private CardObject CreateCard(PlayerType playerType, BaseCard card)
        {
            var newCard = Instantiate(CardObjectPrefab, GetDeckOrigin(playerType), Quaternion.identity);
            newCard.Initialize(card);
            newCard.SetSortingLayer("Card");

            CardObjects.Add(newCard);
            return newCard;
        }

        private TerrainObject CreateTerrain(BaseTerrain terrain, SlotObject slotObject)
        {
            var terrainObject = Instantiate(TerrainObjectPrefab, slotObject.transform.position, Quaternion.identity);
            terrainObject.Initialize(terrain);

            TerrainObjects.Add(terrainObject);
            return terrainObject;
        }

        private IEnumerable<SlotObject> GetSlotObjects(PlayerType playerType, IEnumerable<FieldSlot> fieldSlots)
        {
            return SlotObjects.Where(x => fieldSlots.Contains(x.SourceReference)).Concat(ManaSlotObjects.Where(x => x.Owner == playerType));
        }

        private SlotObject GetFieldSlotObject(FieldSlot slot)
        {
            return FieldSlotObjects.Single(x => x.SourceReference == slot);
        }

        private CardObject GetCardObject(BaseCard card)
        {
            return CardObjects.Single(x => x.SourceReference == card);
        }

        private IEnumerable<CardObject> GetCardObjects(IEnumerable<BaseCard> cardReferences)
        {
            return GetObjects(cardReferences).Cast<CardObject>();
        }

        private IEnumerable<BaseUIObject> GetObjects<T>(IEnumerable<T> sourceReferences) where T : ITargetable
        {
            IEnumerable<BaseUIObject> sourceObjects;

            if (typeof(BaseCard).IsAssignableFrom(typeof(T))) {
                sourceObjects = CardObjects;
            }
            else if (typeof(FieldSlot).IsAssignableFrom(typeof(T))) {
                sourceObjects = FieldSlotObjects;
            }
            else if (typeof(BaseTerrain).IsAssignableFrom(typeof(T))) {
                sourceObjects = TerrainObjects;
            }
            else { 
                throw new ArgumentOutOfRangeException(nameof(T), $"Type must sub class of {typeof(BaseCard)}, {typeof(FieldSlot)} or {typeof(BaseTerrain)}");
            }

            foreach (var sourceObject in sourceObjects)
            {
                if (sourceReferences.Any(x => (ITargetable)x == sourceObject.SourceReference))
                {
                    yield return sourceObject;
                }
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

        public void BeginTargeting<T>(
            IEnumerable<T> allowableTargets,
            int count,
            OnTargetsChosen<T> onTargetsChosen,
            SelectionType selectionType) 
            where T : ITargetable
        {
            var validTargets = GetObjects(allowableTargets);

            TargetManager.Begin<T>(
                validTargets,
                count,
                FinishTargeting,
                selectionType);

            // Translates objects to sources and invokes event delegate
            void FinishTargeting(IEnumerable<BaseUIObject> targetedObjects)
            {
                var chosenTargets = targetedObjects.Select(x => x.SourceReference).Cast<T>();

                var events = onTargetsChosen.Invoke(chosenTargets);

                EnqueueEvents(events);
            }
        }

        public void BeginDragAndDrop(
            IEnumerable<BaseCard> draggableCards,
            IEnumerable<FieldSlot> availableFieldSlots,
            PlayerType playingPlayer,
            Func<BaseCard, FieldSlot, IEnumerable<BaseEvent>> onDrop,
            Func<BaseCard, IEnumerable<BaseEvent>> onSacrifice,
            Func<IEnumerable<BaseEvent>> onPass)
        {
            DragAndDropManager.Begin(
                GetCardObjects(draggableCards),
                GetSlotObjects(playingPlayer, availableFieldSlots),
                DroppedInSlot,
                EnterPressed);

            // Translates objects to sources and invokes event delegate
            bool DroppedInSlot(CardObject cardObject, SlotObject slotObject)
            {
                if (slotObject.SlotType == SlotType.Mana)
                {
                    EnqueueEvents(onSacrifice.Invoke(cardObject.CardReference));
                }
                else
                {
                    var droppedEvents = onDrop.Invoke(cardObject.CardReference, slotObject.SlotReference).ToList();
                    EnqueueEvents(droppedEvents);

                    // Check if playing the card has been succesful - better way to do this?
                    if (!droppedEvents.Any(x => x is NewTurnEvent))
                        return false;
                }

                return true;
            }

            void EnterPressed()
            {
                EnqueueEvents(onPass.Invoke());
            }
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

        public void CreateTerrainInSlot(BaseTerrain terrain, FieldSlot slot)
        {
            var slotObject = GetFieldSlotObject(slot);
            var terrainObject = CreateTerrain(terrain, slotObject);
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

        public void MoveThenRemoveCard(Area area, PlayerType playerType, BaseCard card)
        {
            var cardObject = GetCardObject(card);

            Vector2 location;
            switch (area)
            {
                case Area.Deck:
                    location = GetDeckOrigin(playerType);
                    break;
                case Area.Destroyed:
                    location = GetDestroyedOrigin(playerType);
                    break;
                default:
                    location = cardObject.transform.position;
                    break;
            }

            cardObject.SetTargetPosition(location, true);

            CardObjects.Remove(cardObject);
        }

        public void ShowPile(IEnumerable<BaseSource> sources)
        {
            PileManager.ShowPile(sources);
        }
    }
}
