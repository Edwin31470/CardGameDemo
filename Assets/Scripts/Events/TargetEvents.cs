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

        protected BaseCardSelectionEvent(TargetConditions targetConditions, int count, SelectionType selectionType = SelectionType.Neutral)
        {
            TargetConditions = targetConditions;
            Count = count;
            SelectionType = selectionType;
        }

        public override IEnumerable<BaseEvent> Process(UIManager uIManager, Func<PlayerType, Player> getPlayer)
        {
            uIManager.BeginCardSelection(TargetConditions, OverrideTargets, Count, FinishSelection, SelectionType);
            return Enumerable.Empty<BaseEvent>();
        }

        public abstract IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards);
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
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

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                yield return new EliminateCardEvent(card);
            }
        }
    }

    public class CustomMultiTargetEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => Message;
        private string Message { get; }
        private OnFinishSelection CustomFinishSelection { get; }

        public CustomMultiTargetEvent(TargetConditions targetConditions, int count, SelectionType selectionType, string message, OnFinishSelection onFinishSelection)
            : base(targetConditions, count, selectionType)
        {
            Message = message;
            CustomFinishSelection = onFinishSelection;
        }

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> selectedCards) => CustomFinishSelection.Invoke(selectedCards);
    }

    public class CustomSingleTargetEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => Message;
        private string Message { get; set; }
        private Func<BaseCard, IEnumerable<BaseEvent>> CustomFinishSelection { get; }

        public CustomSingleTargetEvent(TargetConditions targetConditions, SelectionType selectionType, string message, Func<BaseCard, IEnumerable<BaseEvent>> finishSelection)
            : base(targetConditions, 1, selectionType)
        {
            Message = message;
            CustomFinishSelection = finishSelection;
        }

        public override IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> targets)
        {
            var target = targets.SingleOrDefault();
            return target != null ? CustomFinishSelection.Invoke(target) : Enumerable.Empty<BaseEvent>();
        }
    };
}
