using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public abstract class BaseCardSelectionEvent : BaseUIInteractionEvent
    {
        protected TargetConditions TargetConditions { get; set; }
        public int Count { get; set; }
        protected SelectionType SelectionType { get; set; }

        public IEnumerable<BaseCard> OverrideTargets { get; set; } // For use with interupt events
        protected BoardState BoardState { get; set; } // Save the board state to be used in TriggerEffect

        protected BaseCardSelectionEvent(TargetConditions targetConditions, int count, SelectionType selectionType = SelectionType.Neutral)
        {
            TargetConditions = targetConditions;
            Count = count;
            SelectionType = selectionType;
        }

        // Begin card selection in UI
        public override IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board)
        {
            BoardState = BoardState;
            var allowableTargets = board.GetMatchingCards(TargetConditions);
            uIManager.BeginCardSelection(allowableTargets, OverrideTargets, Count, FinishSelection, SelectionType);
            return Enumerable.Empty<BaseEvent>();
        }

        // Get targets
        public IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
        {
            return TriggerEffect(selectedCards);
        }


        // Return new events
        public abstract IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards);
    }

    public class RedrawCardsEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => $"{SourcePlayer} Player chooses {Count} cards to redraw";

        protected PlayerType SourcePlayer { get; set; }

        public RedrawCardsEvent(PlayerType sourcePlayer, int count)
            : base(new TargetConditions { PlayerType = sourcePlayer, Area = Area.Hand }, count)
        {
            SourcePlayer = sourcePlayer;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new ReturnToDeckEvent(card);
                yield return new DrawCardEvent(SourcePlayer);
            }
        }
    }

    public abstract class BaseTargetCreaturesEvent : BaseCardSelectionEvent
    {
        public int Value { get; set; }

        protected BaseTargetCreaturesEvent(TargetConditions targetConditions, int count, int value, SelectionType selectionType)
            : base(targetConditions, count, selectionType)
        {
            Value = value;
        }
    }

    public class DamageTargetsEvent : BaseTargetCreaturesEvent
    {
        public override string EventTitle => $"Damage {Count} creatures by {Value}";

        public DamageTargetsEvent(TargetConditions targetConditions, int count, int value)
            : base(targetConditions, count, value, SelectionType.Negative)
        {
            targetConditions.CardType = CardType.Creature;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                yield return new DamageCreatureEvent(card, Value);
            }
        }
    }

    public class FortifyTargetsEvent : BaseTargetCreaturesEvent
    {
        public override string EventTitle => $"Fortify {Count} creatures by {Value}";

        public FortifyTargetsEvent(TargetConditions targetConditions, int count, int value)
            : base(targetConditions, count, value, SelectionType.Positive)
        {
            targetConditions.CardType = CardType.Creature;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                yield return new FortifyCreatureEvent(card, Value);
            }
        }
    }

    public class WeakenTargetsEvent : BaseTargetCreaturesEvent
    {
        public override string EventTitle => $"Weaken {Count} creatures by {Value}";

        public WeakenTargetsEvent(TargetConditions targetConditions, int count, int value)
            : base(targetConditions, count, value, SelectionType.Negative)
        {
            targetConditions.CardType = CardType.Creature;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                yield return new WeakenCreatureEvent(card, Value);
            }
        }
    }

    public class StrengthenTargetsEvent : BaseTargetCreaturesEvent
    {
        public override string EventTitle => $"Strengthen {Count} creatures by {Value}";

        public StrengthenTargetsEvent(TargetConditions targetConditions, int count, int value)
            : base(targetConditions, count, value, SelectionType.Positive)
        {
            targetConditions.CardType = CardType.Creature;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                yield return new StrengthenCreatureEvent(card, Value);
            }
        }
    }

    public class AddStatsTargetsEvent : BaseTargetCreaturesEvent
    {
        public override string EventTitle => $"Add {Attack}/{Defence} to {Count} creatures";

        public int Attack { get; set; }
        public int Defence { get; set; }

        public AddStatsTargetsEvent(TargetConditions targetConditions, int count, int attack, int defence, SelectionType selectionType = SelectionType.Neutral)
            : base(targetConditions, count, attack + defence, selectionType)
        {
            targetConditions.CardType = CardType.Creature;
            Attack = attack;
            Defence = defence;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                yield return new StrengthenCreatureEvent(card, Attack);
                yield return new FortifyCreatureEvent(card, Defence);
            }
        }
    }

    public class DestroyTargetsEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => $"Destroy {Count} creatures";

        public DestroyTargetsEvent(TargetConditions targetConditions, int count) : base(targetConditions, count, SelectionType.Negative)
        {
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new DestroyCardEvent(card);
            }
        }
    }

    public class EliminateTargetsEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => $"Eliminate {Count} cards";

        public EliminateTargetsEvent(TargetConditions targetConditions, int count) : base(targetConditions, count, SelectionType.Negative)
        {
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new EliminateCardEvent(card);
            }
        }
    }

    // Keep track of source card
    public abstract class BaseCustomTargetEvent<T> : BaseCardSelectionEvent where T : BaseCard
    {
        protected T Source { get; set; }
        public override string EventTitle => Message;
        private string Message { get; }

        protected BaseCustomTargetEvent(T source, TargetConditions targetConditions, int count, SelectionType selectionType, string message) : base(targetConditions, count, selectionType)
        {
            Source = source;
            Message = message;
        }
    }

    public class CustomMultiTargetEvent<T> : BaseCustomTargetEvent<T> where T : BaseCard
    {
        private SelectMultipleTargets<T> CustomFinishSelection { get; }

        public CustomMultiTargetEvent(T source, TargetConditions targetConditions, int count, SelectMultipleTargets<T> onFinishSelection, SelectionType selectionType, string message)
            : base(source, targetConditions, count, selectionType, message)
        {
            CustomFinishSelection = onFinishSelection;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> selectedCards)
        {
            return CustomFinishSelection.Invoke(Source, BoardState, selectedCards);
        }
    }

    public class CustomSingleTargetEvent<T> : BaseCustomTargetEvent<T> where T : BaseCard
    {
        private SelectSingleTarget<T> CustomFinishSelection { get; }

        public CustomSingleTargetEvent(T source, TargetConditions targetConditions, SelectSingleTarget<T> onFinishSelection, SelectionType selectionType, string message)
            : base(source, targetConditions, 1, selectionType, message)
        {
            CustomFinishSelection = onFinishSelection;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<BaseCard> targets)
        {
            var target = targets.SingleOrDefault();
            return target != null ? CustomFinishSelection.Invoke(Source, BoardState, target) : Enumerable.Empty<BaseEvent>();
        }
    };
}
