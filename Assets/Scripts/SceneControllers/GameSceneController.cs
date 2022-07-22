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
using Assets.Scripts.Bases;
using Assets.Scripts.Tokens;

namespace Assets.Scripts
{
    public class GameSceneController : MonoBehaviour
    {
        // Managers
        private UIManager UIManager { get; set; }
        private LabelManager LabelManager { get; set; }

        // Queues and collections
        private RepeatingTimer Timer { get; set; }
        private EventQueue<BaseUIEvent> UIQueue { get; set; } = new();
        private EventQueue<BaseGameEndEvent> GameEndQueue { get; set; } = new();
        private EventQueue<IOnceEvent> EventQueue { get; set; } = new();
        private EventQueue<BasePhaseEvent> PhaseQueue { get; set; } = new();
        private List<IPassiveEvent> PassiveEvents { get; set; } = new();
        private List<IInteruptEvent> InteruptEvents { get; set; } = new();
        private List<ITriggerEvent> TriggerEvents { get; set; } = new();

        // Board
        private BoardState Board { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            // Register Players
            var frontPlayer = new Player(PlayersManager.GetPlayer(PlayerType.Front));
            var backPlayer = new Player(PlayersManager.GetPlayer(PlayerType.Back));

            Board = new BoardState(frontPlayer, backPlayer);

            // Register GameObject Managers
            UIManager = gameObject.AddComponent<UIManager>();
            UIManager.Initialize(Board, EnqueueEvents);

            LabelManager = gameObject.AddComponent<LabelManager>();
            LabelManager.Initialize(frontPlayer, backPlayer);

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
                .onClick.AddListener(() => ShowTokens(PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/TotalAttack").GetComponent<Button>()
                .onClick.AddListener(() => ShowTokens(PlayerType.Back));

            GameObject.Find("Canvas/FrontPlayer/TotalDefence").GetComponent<Button>()
                .onClick.AddListener(() => ShowItems(PlayerType.Front));
            GameObject.Find("Canvas/BackPlayer/TotalDefence").GetComponent<Button>()
                .onClick.AddListener(() => ShowItems(PlayerType.Back));

            // Register Timer
            Timer = gameObject.AddComponent<RepeatingTimer>();
            Timer.Initialize(0.1f, NextEvent);

            EnqueueEvent(new NewPhaseEvent(Phase.GameStart));
            Timer.StartTimer();
        }

        // Main Scene methods (buttons etc.)
        public void ShowPile(Area area, PlayerType playerType)
        {
            var player = Board.GetPlayer(playerType);

            IEnumerable<BaseSource> sources;

            switch (area)
            {
                case Area.Deck:
                    sources = player.Deck
                    .OrderBy(x => x.Colour)
                    .ThenBy(x => x.Cost)
                    .ThenBy(x => x.Type)
                    .ThenBy(x => x.Name)
                    .ToList();
                    break;
                case Area.Destroyed:
                    sources = player.Destroyed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(area), $"Must be {Area.Deck} or {Area.Destroyed}");
            }

            UIManager.ShowPile(sources);
        }

        public void ShowTokens(PlayerType playerType)
        {
            var player = Board.GetPlayer(playerType);

            var tokens = player.Tokens;

            UIManager.ShowPile(tokens);
        }

        public void ShowItems(PlayerType playerType)
        {
            var player = Board.GetPlayer(playerType);

            var items = player.Items;

            UIManager.ShowPile(items);
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
            else if (baseEvent is IPassiveEvent passiveEvent)
            {
                PassiveEvents.Add(passiveEvent);
            }
            else if (baseEvent is BasePhaseEvent phaseEvent)
            {
                PhaseQueue.Enqueue(phaseEvent);
            }
            else if (baseEvent is IOnceEvent onceEvent)
            {
                EventQueue.Enqueue(onceEvent);
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
            //HoverManager.Activated = true;

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
            //HoverManager.Activated = false;

            foreach (var player in Board.BothPlayers)
            {
                var damageTaken = Math.Max(Board.GetPlayer(player.PlayerType.Opposite()).TotalAttack - player.TotalDefence, 0);
                EnqueueEvent(new DamagePlayerEvent(player, damageTaken));
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
            var onceEvent = EventQueue.Dequeue();
            if (onceEvent != null)
            {
                ProcessInteruptEvents(onceEvent);

                ProcessOnceEvent(onceEvent);

                ProcessTriggerEvents(onceEvent);

                return (BaseEvent)onceEvent;
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
            var allCreatureCards = Board.GetMatchingTargets<CreatureCard>(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.PlayArea
            });

            var fieldCreatureCards = Board.GetMatchingTargets<CreatureCard>(new TargetConditions
            {
                CardType = CardType.Creature,
                Area = Area.Field
            });

            // Process passive events
            foreach (var creatureCard in allCreatureCards)
            {
                creatureCard.BonusAttack.Set(0);
                creatureCard.BonusDefence.Set(0);
            }

            foreach (var passiveEvent in PassiveEvents.ToList())
            {
                if (passiveEvent.IsPrevented)
                    continue;

                if (!passiveEvent.IsValid(Board))
                {
                    PassiveEvents.Remove(passiveEvent);
                    continue;
                }

                passiveEvent.Process(Board);
            }

            // Check if passive events have caused any creatures to die
            foreach (var creatureCard in fieldCreatureCards)
            {
                if(creatureCard.Defence <= 0)
                {
                    EnqueueEvent(new DestroyByDamageEvent(creatureCard));
                }
            }

            // Check if any tokens should be destroyed (tokens must have at least one event present (except stat tokens))
            foreach (var player in Board.BothPlayers)
            {
                foreach (var token in player.Tokens.Where(x => !x.IsBasicToken()).ToList())
                {
                    if (PassiveEvents.Any(x => x.BaseSource == token))
                        continue;

                    if (InteruptEvents.Any(x => x.BaseSource == token))
                        continue;

                    if (TriggerEvents.Any(x => x.BaseSource == token))
                        continue;

                    player.Tokens.Remove(token);
                }
            }

            // Check if Interupt events, Trigger events or Passive events should be interupted or prevented
            foreach (var interuptEvent in InteruptEvents.ToArray())
            {
                interuptEvent.IsPrevented = false;
                ProcessInteruptEvents(interuptEvent);
            }

            foreach (var passiveEvent in PassiveEvents)
            {
                passiveEvent.IsPrevented = false;
                ProcessInteruptEvents(passiveEvent);
            }

            foreach (var triggerEvent in TriggerEvents)
            {
                triggerEvent.IsPrevented = false;
                ProcessInteruptEvents(triggerEvent);
            }
        }

        private void ProcessInteruptEvents(IInteruptableEvent interuptingEvent)
        {
            foreach (var interuptEvent in InteruptEvents.ToList())
            {
                if (interuptingEvent == interuptEvent)
                    continue;

                TryInterupt(interuptingEvent, interuptEvent);
            }
        }

        private bool TryInterupt(IInteruptableEvent interuptedEvent, IInteruptEvent interuptingEvent)
        {
            if (!interuptingEvent.IsValid(Board)) {
                InteruptEvents.Remove(interuptingEvent);
                return false;
            }

            if (interuptingEvent.IsPrevented)
                return false;

            var wasInterupted = interuptingEvent.Process(Board, interuptedEvent);

            if (wasInterupted && interuptingEvent.TriggerOnce)
                InteruptEvents.Remove(interuptingEvent);

            return wasInterupted;
        }

        private void ProcessOnceEvent(IOnceEvent onceEvent)
        {
            if (onceEvent.IsPrevented)
                return;

            IEnumerable<BaseEvent> newEvents;

            switch (onceEvent)
            {
                case IUIInteractionEvent uIInteractionEvent:
                    newEvents = uIInteractionEvent.Process(UIManager, Board);
                    break;
                case IGameplayEvent boardEvent:
                    newEvents = boardEvent.Process(Board);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(onceEvent), $"Event must be either {typeof(IUIInteractionEvent)} or {typeof(IGameplayEvent)}");
            }

            EnqueueEvents(newEvents);
        }

        private void ProcessTriggerEvents(ITriggeringEvent triggeringEvent)
        {
            foreach(var triggerEvent in TriggerEvents.ToList())
            {
                if (triggerEvent.IsPrevented)
                    return;

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