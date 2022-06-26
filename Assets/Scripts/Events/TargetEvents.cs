using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public abstract class BaseCardSelectionEvent<TSource, TTarget> : BaseUIInteractionEvent<TSource>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        protected TargetConditions TargetConditions { get; set; }
        public int Count { get; set; }
        protected SelectionType SelectionType { get; set; }

        public IEnumerable<TTarget> OverrideTargets { get; set; } // For use with interupt events
        protected BoardState BoardState { get; set; } // Save the board state to be used in TriggerEffect

        protected BaseCardSelectionEvent(TSource source, TargetConditions targetConditions, int count, SelectionType selectionType = SelectionType.Neutral) : base(source)
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
            var v = selectedCards.Cast<TTarget>().ToList();
            return TriggerEffect(selectedCards.Cast<TTarget>());
        }


        // Return new events
        public abstract IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> selectedCards);
    }

    public class RedrawCardsEvent<TSource, TTarget> : BaseCardSelectionEvent<TSource, TTarget> 
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        public override string EventTitle => $"{PlayerType} Player chooses {Count} cards to redraw";

        PlayerType PlayerType { get; set; }

        public RedrawCardsEvent(TSource source, PlayerType playerType, int count)
            : base(source, new TargetConditions { PlayerType = playerType, Area = Area.Hand }, count)
        {
            PlayerType = playerType;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new ReturnToDeckEvent(card);
                yield return new DrawCardEvent(PlayerType);
            }
        }
    }

    public abstract class BaseTargetCreaturesEvent<TSource> : BaseCardSelectionEvent<TSource, CreatureCard> where TSource : BaseSource
    {
        protected BaseTargetCreaturesEvent(TSource source, TargetConditions targetConditions, int count, SelectionType selectionType)
            : base(source, targetConditions, count, selectionType)
        {
            targetConditions.CardType = CardType.Creature;
        }
    }

    public abstract class BaseTargetStatEvent<TSource, TStatEvent> : BaseTargetCreaturesEvent<TSource>
        where TSource : BaseSource
        where TStatEvent : BaseStatEvent<TSource>
    {
        public int Value { get; set; }

        protected BaseTargetStatEvent(TSource source, TargetConditions targetConditions, int count, int value, SelectionType selectionType)
            : base(source, targetConditions, count, selectionType)
        {
            targetConditions.CardType = CardType.Creature;
            Value = value;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<CreatureCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return (TStatEvent)Activator.CreateInstance(typeof(TStatEvent), Source, card, Value);
            }
        }
    }

    public class DamageTargetsEvent<T> : BaseTargetStatEvent<T, DamageCreatureEvent<T>> where T : BaseSource
    {
        public override string EventTitle => $"Damage {Count} creatures by {Value}";

        public DamageTargetsEvent(T source, TargetConditions targetConditions, int count, int value)
            : base(source, targetConditions, count, value, SelectionType.Negative)
        {
        }
    }

    public class FortifyTargetsEvent<T> : BaseTargetStatEvent<T, DamageCreatureEvent<T>> where T : BaseSource
    {
        public override string EventTitle => $"Fortify {Count} creatures by {Value}";

        public FortifyTargetsEvent(T source, TargetConditions targetConditions, int count, int value)
            : base(source, targetConditions, count, value, SelectionType.Positive)
        {
        }
    }

    public class WeakenTargetsEvent<T> : BaseTargetStatEvent<T, DamageCreatureEvent<T>> where T : BaseSource
    {
        public override string EventTitle => $"Weaken {Count} creatures by {Value}";

        public WeakenTargetsEvent(T source, TargetConditions targetConditions, int count, int value)
            : base(source, targetConditions, count, value, SelectionType.Negative)
        {
        }
    }

    public class StrengthenTargetsEvent<T> : BaseTargetStatEvent<T, DamageCreatureEvent<T>> where T : BaseSource
    {
        public override string EventTitle => $"Strengthen {Count} creatures by {Value}";

        public StrengthenTargetsEvent(T source, TargetConditions targetConditions, int count, int value)
            : base(source, targetConditions, count, value, SelectionType.Positive)
        {
        }
    }

    public class AddStatsTargetsEvent<T> : BaseCardSelectionEvent<T, CreatureCard> where T : BaseSource
    {
        public override string EventTitle => $"Add {Attack}/{Defence} to {Count} creatures";

        public int Attack { get; set; }
        public int Defence { get; set; }

        public AddStatsTargetsEvent(T source, TargetConditions targetConditions, int count, int attack, int defence, SelectionType selectionType = SelectionType.Neutral)
            : base(source, targetConditions, count, selectionType)
        {
            targetConditions.CardType = CardType.Creature;
            Attack = attack;
            Defence = defence;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<CreatureCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new StrengthenCreatureEvent<BaseSource>(Source, card, Attack);
                yield return new FortifyCreatureEvent<BaseSource>(Source, card, Defence);
            }
        }
    }

    public class DestroyTargetsEvent<TSource, TTarget> : BaseCardSelectionEvent<TSource, TTarget>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        public override string EventTitle => $"Destroy {Count} cards";

        public DestroyTargetsEvent(TSource source, TargetConditions targetConditions, int count) : base(source, targetConditions, count, SelectionType.Negative)
        {
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new DestroyCardEvent(card);
            }
        }
    }

    public class EliminateTargetsEvent<TSource, TTarget> : BaseCardSelectionEvent<TSource, TTarget>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        public override string EventTitle => $"Eliminate {Count} cards";

        public EliminateTargetsEvent(TSource source, TargetConditions targetConditions, int count) : base(source, targetConditions, count, SelectionType.Negative)
        {
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new EliminateCardEvent(card);
            }
        }
    }

    // Keep track of source card
    public abstract class BaseCustomTargetEvent<TSource, TTarget> : BaseCardSelectionEvent<TSource, TTarget>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        public override string EventTitle => Message;
        private string Message { get; }

        protected BaseCustomTargetEvent(TSource source, TargetConditions targetConditions, int count, SelectionType selectionType, string message) : base(source, targetConditions, count, selectionType)
        {
            Source = source;
            Message = message;
        }
    }

    public class CustomMultiTargetEvent<TSource, TTarget> : BaseCustomTargetEvent<TSource, TTarget>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        private SelectMultipleTargets<TSource, TTarget> CustomFinishSelection { get; }

        public CustomMultiTargetEvent(TSource source, TargetConditions targetConditions, int count, SelectMultipleTargets<TSource, TTarget> onFinishSelection, SelectionType selectionType, string message)
            : base(source, targetConditions, count, selectionType, message)
        {
            CustomFinishSelection = onFinishSelection;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> selectedCards)
        {
            return CustomFinishSelection.Invoke((TSource)Source, BoardState, selectedCards);
        }
    }

    public class CustomSingleTargetEvent<TSource, TTarget> : BaseCustomTargetEvent<TSource, TTarget>
        where TSource : BaseSource
        where TTarget : BaseCard
    {
        private SelectSingleTarget<TSource, TTarget> CustomFinishSelection { get; }

        public CustomSingleTargetEvent(TSource source, TargetConditions targetConditions, SelectSingleTarget<TSource, TTarget> onFinishSelection, SelectionType selectionType, string message)
            : base(source, targetConditions, 1, selectionType, message)
        {
            CustomFinishSelection = onFinishSelection;
        }

        public override IEnumerable<BaseEvent> TriggerEffect(IEnumerable<TTarget> targets)
        {
            var target = targets.SingleOrDefault();
            return target != null ? CustomFinishSelection.Invoke((TSource)Source, BoardState, target) : Enumerable.Empty<BaseEvent>();
        }
    };
}
