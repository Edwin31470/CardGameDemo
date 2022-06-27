using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Events.Interfaces;

namespace Assets.Scripts
{
    public class MainController : MonoBehaviour
    {
        // Managers
        private UIManager UIManager { get; set; }
        private LabelManager LabelManager { get; set; }
        private PileManager PileManager { get; set; }
        private ShowTokensManager ShowTokensManager { get; set; }

        // Queues and collections
        private RepeatingTimer Timer { get; set; }
        private EventQueue<BaseUIEvent> UIQueue { get; set; } = new();
        private EventQueue<BaseGameEndEvent> GameEndQueue { get; set; } = new();
        private EventQueue<BaseGameplayEvent> EventQueue { get; set; } = new();
        private EventQueue<BasePhaseEvent> PhaseQueue { get; set; } = new();
        private List<BasePassiveEvent> PassiveEvents { get; set; } = new();
        private List<IInteruptEvent> InteruptEvents { get; set; } = new();
        private List<ITriggerEvent> TriggerEvents { get; set; } = new();

        // Board
        private BoardState Board { get; set; }

        // Static method to be used by scripts requiring Update()
        public static MainController Get()
        {
            return GameObject.Find("MainController").GetComponent<MainController>();
        }


        // Start is called before the first frame update
        void Start()
        {
            // Register Players

            var frontDeck = DeckManager.GetDeck("RedDeck");
            var backDeck = DeckManager.GetDeck("RedDeck");

            var frontItems = ItemManager.GetItems(new[] { 1, 3, 4, 5, 7 });
            var backItems = ItemManager.GetItems(new[] { 2 });

            var frontPlayer = new Player(PlayerType.Front, frontDeck, frontItems);
            var backPlayer = new Player(PlayerType.Back, backDeck, backItems);

            Board = new BoardState(frontPlayer, backPlayer);

            // Register GameObject Managers
            UIManager = gameObject.AddComponent(typeof(UIManager)) as UIManager;
            UIManager.Initialize(Board);

            LabelManager = gameObject.AddComponent(typeof(LabelManager)) as LabelManager;
            LabelManager.Initialize(frontPlayer, backPlayer);

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

            // Register Timer
            Timer = gameObject.AddComponent(typeof(RepeatingTimer)) as RepeatingTimer;
            Timer.Initialize(0.1f, NextEvent);

            EnqueueEvent(new NewPhaseEvent(Phase.GameStart));
            Timer.StartTimer();
        }

        // Main Scene methods (buttons etc.)
        public void ShowPile(Area area, PlayerType playerType)
        {
            var player = Board.GetPlayer(playerType);

            var cards = new List<BaseCard>();

            if (area == Area.Deck)
                cards = player.Deck.OrderBy(x => x.Colour).ThenBy(x => x.Cost).ThenBy(x => x.Type).ThenBy(x => x.Name).ToList();
            else if (area == Area.Destroyed)
                cards = player.Destroyed;

            PileManager.ShowPile(cards);
        }

        public void ShowTokens(StatType statType, PlayerType playerType)
        {
            var player = Board.GetPlayer(playerType);

            var tokens = player.GetTokens(statType);

            ShowTokensManager.ShowTokens(tokens);
        }

        public void EnqueueEvent(BaseEvent baseEvent)
        {
            if (baseEvent is BaseUIEvent uiEvent)
            {
                UIQueue.Enqueue(uiEvent);
            }
            else if (baseEvent is BaseGameEndEvent gameEndEvent)
            {
                GameEndQueue.Enqueue(gameEndEvent);
            }
            else if (baseEvent is IInteruptEvent interuptEvent)
            {
                InteruptEvents.Add(interuptEvent);
            }
            else if (baseEvent is ITriggerEvent triggerEvent)
            {
                TriggerEvents.Add(triggerEvent);
            }
            else if (baseEvent is BasePhaseEvent phaseEvent)
            {
                PhaseQueue.Enqueue(phaseEvent);
            }
            else if (baseEvent is BaseGameplayEvent gameplayEvent)
            {
                EventQueue.Enqueue(gameplayEvent);
            }
            else if (baseEvent is BasePassiveEvent passiveEvent)
            {
                PassiveEvents.Add(passiveEvent);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(baseEvent), "Event not queued, something is wrong with the inheritance of events");
            }
        }

        public void EnqueueEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                EnqueueEvent(@event);
            }
        }

        public void NewPhase(Phase phase)
        {
            Board.CurrentPhase = phase;

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), "Phase not valid, must be a playable phase");
            }
        }

        private void StartGame()
        {
            // Get item effects
            foreach(var player in Board.BothPlayers)
            {
                foreach(var item in player.Items)
                {
                    var events = item.GetEvents(Board);
                    EnqueueEvents(events);
                }
            }

            EnqueueEvent(new NewPhaseEvent(Phase.Draw));
        }

        private void DrawPhase()
        {
            Board.RoundNumber++;
            Timer.SetTickLength(0.1f);

            for (int i = 0; i < 7; i++)
            {
                foreach(var player in Board.BothPlayers)
                {
                    EnqueueEvent(new DrawCardEvent<Player>(player, player.PlayerType));
                }
            }

            EnqueueEvent(new NewPhaseEvent(Phase.Redraw));
        }

        private void RedrawPhase()
        {
            //Timer.SetTickLength(0.5f);

            foreach(var player in Board.BothPlayers)
            {
                EnqueueEvent(new RedrawCardsEvent<Player, BaseCard>(player, player.PlayerType, 3));
            }

            EnqueueEvent(new NewPhaseEvent(Phase.Play));
        }

        private void PlayPhase()
        {
            EnqueueEvent(new NewTurnEvent(PlayerType.Front, false));
        }

        private void DamagePhase()
        {
            foreach (var player in Board.BothPlayers)
            {
                var damageTaken = Math.Max(Board.GetPlayer(player.PlayerType.Opposite()).TotalAttack - player.TotalDefence, 0);
                EnqueueEvent(new DamagePlayerEvent(player.PlayerType, damageTaken));
            }

            EnqueueEvent(new NewPhaseEvent(Phase.Mana));
        }

        private void ManaPhase()
        {
            Timer.SetTickLength(0.1f);

            // Clear Board
            foreach (var player in Board.BothPlayers)
            {
                foreach (var card in player.FieldCards)
                {
                    if (card.HasPersistence)
                        continue;

                    EnqueueEvent(new DestroyCardEvent<FieldCard>(card));
                }
            }

            // Return Hand to Deck
            foreach (var player in Board.BothPlayers)
            {
                foreach (var card in player.Hand)
                {
                    EnqueueEvent(new ReturnToDeckEvent<Player>(player, card));
                }
            }

            // Empty Destroyed
            foreach (var player in Board.BothPlayers)
            {
                EnqueueEvent(new EmptyDestroyPileEvent(player));
            }

            EnqueueEvent(new NewPhaseEvent(Phase.Draw));
        }

        private void EndGame(string message)
        {
            // Empty all queues
            GameEndQueue.Empty();
            UIQueue.Empty();
            EventQueue.Empty();
            PhaseQueue.Empty();

            EnqueueEvent(new MessageEvent(message, int.MaxValue));
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

        /// <summary>
        /// Pop and then process the next item in the next available queue
        /// </summary>
        /// <returns>The event that has been processed</returns>
        private BaseEvent ProcessNextEvent()
        {
            // Process the UI queue until empty
            var uIEvent = UIQueue.Dequeue();
            if (uIEvent != null)
            {
                uIEvent.Process(UIManager);
                return uIEvent;
            }

            // Check if the game has ended - might need to go before the UI queue
            var gameEndEvent = GameEndQueue.Dequeue();
            if (gameEndEvent != null)
            {
                var message = gameEndEvent.Process();
                EndGame(message);
                return gameEndEvent;
            }

            // Process the gameplay queue until empty
            var gameplayEvent = EventQueue.Dequeue();
            if (gameplayEvent != null)
            {
                ProcessInteruptEvents(gameplayEvent);

                var newEvents = ProcessGameplayEvent(gameplayEvent);
                EnqueueEvents(newEvents);

                return gameplayEvent;
            }

            // Process end of phase events
            var phaseEvent = PhaseQueue.Dequeue();
            if (phaseEvent != null)
            {
                phaseEvent.Process(this, Board);
                return phaseEvent;
            }

            throw new NotImplementedException("Event queues should never be completely empty");
        }

        private void ProcessBoardState()
        {
            var allCreatureCards = Board.GetMatchingCards(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.PlayArea
            }).OfType<CreatureCard>();

            var fieldCreatureCards = Board.GetMatchingCards(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.Field
            }).OfType<CreatureCard>();

            foreach (var creatureCard in allCreatureCards)
            {
                creatureCard.BonusAttack.Set(0);
                creatureCard.BonusDefence.Set(0);
            }

            foreach (var passiveEvent in PassiveEvents.ToList())
            {
                if (!passiveEvent.IsValid(Board))
                {
                    PassiveEvents.Remove(passiveEvent);
                    continue;
                }

                passiveEvent.Process(Board);
            }

            foreach (var creatureCard in fieldCreatureCards)
            {
                if(creatureCard.Defence <= 0)
                {
                    EnqueueEvent(new DestroyByDamageEvent(creatureCard));
                }
            }
        }

        private void ProcessInteruptEvents(BaseGameplayEvent baseEvent)
        {
            foreach (var interuptEvent in InteruptEvents.ToList())
            {
                if (!interuptEvent.IsValid(Board))
                {
                    InteruptEvents.Remove(interuptEvent);
                    continue;
                }

                var interupted = interuptEvent.Process(Board, baseEvent);

                if (interupted && interuptEvent.TriggerOnce)
                    InteruptEvents.Remove(interuptEvent);
            }
        }

        private IEnumerable<BaseEvent> ProcessGameplayEvent(BaseGameplayEvent gameplayEvent)
        {
            switch (gameplayEvent)
            {
                case IUIInteractionEvent uIInteractionEvent:
                    return uIInteractionEvent.Process(UIManager, Board);
                case BaseBoardEvent boardEvent:
                    return boardEvent.Process(Board);
                default:
                    return gameplayEvent.Process();
            }
        }

        private void ProcessTriggerEvents(BaseEvent triggeringEvent)
        {
            foreach(var triggerEvent in TriggerEvents.ToList())
            {
                if (triggerEvent.Conditions(Board, triggeringEvent))
                {
                    if (triggerEvent.TriggerOnce || !triggerEvent.IsValid(Board)) {
                        TriggerEvents.Remove(triggerEvent);
                    }

                    var newEvents = triggerEvent.Process(Board, triggeringEvent);
                    EnqueueEvents(newEvents);
                }
            }
        }
    }
}