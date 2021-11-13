using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainController : MonoBehaviour
    {
        private DeckManager DeckManager { get; set; }
        private UIManager UIManager { get; set; }
        private LabelManager LabelManager { get; set; }
        private PileManager PileManager { get; set; }
        private ShowTokensManager ShowTokensManager { get; set; }


        private Player FrontPlayer { get; set; }
        private Player BackPlayer { get; set; }

        private EventQueue<BaseGameplayEvent> EventQueue { get; set; }
        private EventQueue<BasePhaseEvent> PhaseQueue { get; set; }
        private EventQueue<BaseUIEvent> UIQueue { get; set; }
        private List<BasePassiveEvent> PassiveEvents { get; set; }
        private List<BaseInteruptEvent> InteruptEvents { get; set; }
        private List<BaseTriggerEvent> TriggerEvents { get; set; }

        private RepeatingTimer Timer { get; set; }

        private int RoundNumber { get; set; }
        private Phase CurrentPhase { get; set; }

        // Helper Methods
        public static Player GetPlayer(PlayerType playerType)
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            if (playerType == PlayerType.Front)
                return controller.FrontPlayer;
            else if (playerType == PlayerType.Back)
                return controller.BackPlayer;
            else
                throw new ArgumentOutOfRangeException("playerType", $"Must be either {PlayerType.Front} or {PlayerType.Back}");
        }

        public static Phase GetCurrentPhase()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.CurrentPhase;
        }

        public static int GetRoundNumber()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.RoundNumber;
        }

        public static HashSet<T> GetCardsInPlay<T>(TargetConditions targetConditions) where T : BaseCard
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            var frontHandCards = controller.FrontPlayer.GetHand().Where(x => targetConditions.IsMatch(x));
            var frontFieldCards = controller.FrontPlayer.GetField().Where(x => targetConditions.IsMatch(x));
            var backHandCards = controller.BackPlayer.GetHand().Where(x => targetConditions.IsMatch(x));
            var backFieldCards = controller.BackPlayer.GetField().Where(x => targetConditions.IsMatch(x));

            var allCards = frontHandCards.Concat(frontFieldCards).Concat(backHandCards).Concat(backFieldCards);

            return new HashSet<T>(allCards.Cast<T>());
        }

        public static EventQueue<BaseGameplayEvent> GetEventQueue()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.EventQueue;
        }

        public static List<BasePassiveEvent> GetPassiveEvents()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.PassiveEvents;
        }

        public static List<BaseInteruptEvent> GetInteruptEvents()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.InteruptEvents;
        }

        public static List<BaseTriggerEvent> GetTriggerEvents()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            return controller.TriggerEvents;
        }

        public static void ClearPhaseQueue()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            while (!controller.PhaseQueue.IsEmpty)
            {
                controller.PhaseQueue.DequeueEvent();
            }
        }

        public static void ClearGameplayQueue()
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            while (!controller.EventQueue.IsEmpty)
            {
                controller.EventQueue.DequeueEvent();
            }
        }

        public static void AddEvent(BaseEvent baseEvent)
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            controller.AddEventToQueue(baseEvent);
        }

        public static void RemoveEvent(BaseEvent baseEvent)
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();

            if (baseEvent is BasePassiveEvent passiveEvent)
            {
                controller.PassiveEvents.Remove(passiveEvent);
            }
            else if (baseEvent is BaseInteruptEvent interuptEvent)
            {
                controller.InteruptEvents.Remove(interuptEvent);
            }
            else if (baseEvent is BaseTriggerEvent triggerEvent)
            {
                controller.TriggerEvents.Remove(triggerEvent);
            }
            else
            {
                throw new ArgumentOutOfRangeException("baseEvent", $"{baseEvent.GetType()} is not a valid event to be removed");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Register Players
            DeckManager = new DeckManager();

            var frontDeck = DeckManager.GetDeck("Blue Deck");
            var backDeck = DeckManager.GetDeck("Purple Deck");

            FrontPlayer = new Player(PlayerType.Front, frontDeck);
            BackPlayer = new Player(PlayerType.Back, backDeck);


            // Register GameObject Managers
            UIManager = gameObject.AddComponent(typeof(UIManager)) as UIManager;
            LabelManager = gameObject.AddComponent(typeof(LabelManager)) as LabelManager;
            LabelManager.Initialize(FrontPlayer, BackPlayer);
            PileManager = gameObject.AddComponent(typeof(PileManager)) as PileManager;
            ShowTokensManager = gameObject.AddComponent(typeof(ShowTokensManager)) as ShowTokensManager;


            // Register Buttons
            GameObject.Find("Canvas/FrontPlayer/DestroyedCount").GetComponent<Button>()
                .onClick.AddListener(() => ShowPile(Area.Destroyed, PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/DestroyedCount").GetComponent<Button>()
                .onClick.AddListener(() => ShowPile(Area.Destroyed, PlayerType.Back));

            GameObject.Find("Canvas/FrontPlayer/DeckCount").GetComponent<Button>()
                .onClick.AddListener(() => ShowPile(Area.Deck, PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/DeckCount").GetComponent<Button>()
                .onClick.AddListener(() => ShowPile(Area.Deck, PlayerType.Back));

            GameObject.Find("Canvas/FrontPlayer/TotalAttack").GetComponent<Button>()
                .onClick.AddListener(() => ShowTokens(StatType.Attack, PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/TotalAttack").GetComponent<Button>()
                .onClick.AddListener(() => ShowTokens(StatType.Attack, PlayerType.Back));

            GameObject.Find("Canvas/FrontPlayer/TotalDefence").GetComponent<Button>()
                .onClick.AddListener(() => ShowTokens(StatType.Defence, PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/TotalDefence").GetComponent<Button>()
                .onClick.AddListener(() => ShowTokens(StatType.Defence, PlayerType.Back));


            // Register Event Queues
            EventQueue = new EventQueue<BaseGameplayEvent>();
            PhaseQueue = new EventQueue<BasePhaseEvent>();
            UIQueue = new EventQueue<BaseUIEvent>();
            PassiveEvents = new List<BasePassiveEvent>();
            InteruptEvents = new List<BaseInteruptEvent>();
            TriggerEvents = new List<BaseTriggerEvent>();


            // Register Timer
            Timer = gameObject.AddComponent(typeof(RepeatingTimer)) as RepeatingTimer;
            Timer.Initialize(0.1f, NextEvent);


            AddEventToQueue(new NewPhaseEvent(Phase.GameStart));
            Timer.StartTimer();
        }

        // Main Scene methods (buttons etc.)
        public void ShowPile(Area area, PlayerType playerType)
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();
            var player = GetPlayer(playerType);

            var cards = new List<BaseCard>();

            if (area == Area.Deck)
                cards = player.GetDeck().OrderBy(x => x.Colour).ThenBy(x => x.Cost).ThenBy(x => x.Type).ThenBy(x => x.Name).ToList();
            else if (area == Area.Destroyed)
                cards = player.GetDestroyed();

            controller.PileManager.ShowPile(cards);
        }

        public void ShowTokens(StatType statType, PlayerType playerType)
        {
            var controller = GameObject.Find("MainController").GetComponent<MainController>();
            var player = GetPlayer(playerType);

            var tokens = player.GetTokens(statType);

            controller.ShowTokensManager.ShowTokens(tokens);
        }

        private void AddEventToQueue(BaseEvent baseEvent)
        {
            if (baseEvent is BaseUIEvent uiEvent)
            {
                UIQueue.EnqueueEvent(uiEvent);
            }
            else if (baseEvent is BasePhaseEvent phaseEvent)
            {
                PhaseQueue.EnqueueEvent(phaseEvent);
            }
            else if (baseEvent is BaseGameplayEvent gameplayEvent)
            {
                EventQueue.EnqueueEvent(gameplayEvent);
            }
            else if (baseEvent is BasePassiveEvent passiveEvent)
            {
                PassiveEvents.Add(passiveEvent);
            }
            else if (baseEvent is BaseInteruptEvent interuptEvent)
            {
                InteruptEvents.Add(interuptEvent);
            }
            else if (baseEvent is BaseTriggerEvent triggerEvent)
            {
                TriggerEvents.Add(triggerEvent);
            }
            else
            {
                throw new ArgumentOutOfRangeException("baseEvent", "Event not queued, something is wrong with the inheritance of events");
            }
        }

        public void NewPhase(Phase phase)
        {
            CurrentPhase = phase;

            switch (phase)
            {
                case Phase.GameStart:
                    StartGame();
                    break;
                case Phase.Draw:
                    DrawPhase();
                    break;
                case Phase.Redraw:
                    RedrawPhase();
                    break;
                case Phase.Play:
                    PlayPhase();
                    break;
                case Phase.Damage:
                    DamagePhase();
                    break;
                case Phase.Mana:
                    ManaPhase();
                    break;
                case Phase.GameEnd:
                    EndGame();
                    break;
            }
        }

        private void StartGame()
        {
            AddEventToQueue(new NewPhaseEvent(Phase.Draw));
        }

        private void DrawPhase()
        {
            RoundNumber++;

            for (int i = 0; i < 7; i++)
            {
                AddEventToQueue(new DrawCardEvent(PlayerType.Front));
                AddEventToQueue(new DrawCardEvent(PlayerType.Back));
            }

            AddEventToQueue(new NewPhaseEvent(Phase.Redraw));
        }

        private void RedrawPhase()
        {
            //Timer.SetTickLength(0.5f);

            AddEventToQueue(new RedrawCardsEvent(PlayerType.Front, 3));
            AddEventToQueue(new RedrawCardsEvent(PlayerType.Back, 3));

            AddEventToQueue(new NewPhaseEvent(Phase.Play));
        }

        private void PlayPhase()
        {
            AddEventToQueue(new NewTurnEvent(PlayerType.Front, false));
        }

        private void DamagePhase()
        {
            var frontTaken = Math.Max(BackPlayer.TotalAttack - FrontPlayer.TotalDefence, 0);
            var backTaken = Math.Max(FrontPlayer.TotalAttack - BackPlayer.TotalDefence, 0);

            AddEventToQueue(new DamagePlayerEvent(PlayerType.Front, frontTaken));
            AddEventToQueue(new DamagePlayerEvent(PlayerType.Back, backTaken));

            AddEventToQueue(new NewPhaseEvent(Phase.Mana));
        }

        private void ManaPhase()
        {
            Timer.SetTickLength(0.1f);

            // Clear Board
            foreach (var card in FrontPlayer.GetField())
            {
                if (card.HasPersistence)
                    continue;

                AddEventToQueue(new DestroyCardEvent(card));
            }

            foreach (var card in BackPlayer.GetField())
            {
                if (card.HasPersistence)
                    continue;

                AddEventToQueue(new DestroyCardEvent(card));
            }


            // Return Hand to Deck
            foreach (var card in FrontPlayer.GetHand())
            {
                AddEventToQueue(new ReturnToDeckEvent(card));
            }

            foreach (var card in BackPlayer.GetHand())
            {
                AddEventToQueue(new ReturnToDeckEvent(card));
            }


            // Empty Destroyed
            AddEventToQueue(new EmptyDestroyPileEvent(FrontPlayer.PlayerType));
            AddEventToQueue(new EmptyDestroyPileEvent(BackPlayer.PlayerType));


            AddEventToQueue(new NewPhaseEvent(Phase.Draw));
        }

        private void EndGame()
        {
            var frontPlayerHealth = FrontPlayer.Health.Get();
            var backPlayerHealth = BackPlayer.Health.Get();
            var lowestHealthPlayer = frontPlayerHealth <= backPlayerHealth ? FrontPlayer : BackPlayer;

            var frontPlayerDeckEmpty = FrontPlayer.DeckCount() == 0;
            var backPlayerDeckEmpty = BackPlayer.DeckCount() == 0;

            if (lowestHealthPlayer.Health.Get() <= 0)
            {
                AddEventToQueue(new MessageEvent($"{lowestHealthPlayer.PlayerType} Player has run out of life!", 10000));
            }
            else if (frontPlayerDeckEmpty && lowestHealthPlayer.PlayerType == PlayerType.Front)
            {
                AddEventToQueue(new MessageEvent($"{PlayerType.Front} Player has run out of cards!", 10000));
            }
            else if (backPlayerDeckEmpty && lowestHealthPlayer.PlayerType == PlayerType.Back)
            {
                AddEventToQueue(new MessageEvent($"{PlayerType.Back} Player has run out of cards!", 10000));
            }
        }



        private void NextEvent()
        {
            // Wait for the UIManager to finish processing
            if (UIManager.IsProcessing)
                return;

            ProcessBoardState();
            var baseEvent = ProcessNextEvent();
            ProcessTriggerEvents(baseEvent);

            LabelManager.SetCurrentEvent(baseEvent.EventTitle);

            if (baseEvent.Delay != 0)
            {
                Timer.AddDelay(baseEvent.Delay);
            }
        }

        private BaseEvent ProcessNextEvent()
        {
            // Process the UI queue until empty
            var uIEvent = UIQueue.DequeueEvent();
            if (uIEvent != null)
            {
                uIEvent.Process(UIManager);
                return uIEvent;
            }

            // Process the gameplay queue until empty
            var gameplayEvent = EventQueue.DequeueEvent();
            if (gameplayEvent != null)
            {
                ProcessInteruptEvents(gameplayEvent);

                if (gameplayEvent is BaseUIInteractionEvent uIInteractionEvent)
                {
                    uIInteractionEvent.Process(UIManager);
                }
                else
                {
                    gameplayEvent.Process();
                }

                return gameplayEvent;
            }

            // Process end of phase events
            var phaseEvent = PhaseQueue.DequeueEvent();
            if (phaseEvent != null)
            {
                phaseEvent.Process(this);
                return phaseEvent;
            }

            throw new NotImplementedException("Event queues should never be completely empty");
        }

        private void ProcessBoardState()
        {
            var allCreatureCards = GetCardsInPlay<CreatureCard>(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.PlayArea
            });

            var fieldCreatureCards = GetCardsInPlay<CreatureCard>(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.Field
            });

            foreach (var creatureCard in allCreatureCards)
            {
                creatureCard.BonusAttack.Set(0);
                creatureCard.BonusDefence.Set(0);
            }

            foreach (var passiveEvent in PassiveEvents.ToList())
            {
                if (!passiveEvent.IsValid())
                {
                    PassiveEvents.Remove(passiveEvent);
                    continue;
                }

                passiveEvent.Process(this);
            }

            foreach (var creatureCard in fieldCreatureCards)
            {
                if(creatureCard.Defence <= 0)
                {
                    AddEvent(new DestroyCreatureByDamageEvent(creatureCard));
                }
            }
        }

        private void ProcessInteruptEvents(BaseGameplayEvent baseEvent)
        {
            foreach (var interuptEvent in InteruptEvents.ToList())
            {
                if (!interuptEvent.IsValid())
                {
                    InteruptEvents.Remove(interuptEvent);
                    continue;
                }

                var interupted = interuptEvent.Process(baseEvent);

                if (interupted && interuptEvent is BaseInteruptOnceEvent)
                    InteruptEvents.Remove(interuptEvent);
            }
        }

        private void ProcessTriggerEvents(BaseEvent triggeringEvent)
        {
            foreach(var triggerEvent in TriggerEvents.ToList())
            {
                var triggered = triggerEvent.Process(triggeringEvent);

                if ((triggered && triggerEvent is BaseTriggerOnceEvent) || !triggerEvent.IsValid())
                    TriggerEvents.Remove(triggerEvent);
            }
        }
    }
}