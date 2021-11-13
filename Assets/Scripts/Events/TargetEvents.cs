using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void Process(UIManager uIManager)
        {
            uIManager.BeginCardSelection(TargetConditions, OverrideTargets, Count, FinishedSelection, SelectionType);
        }

        public abstract void FinishedSelection(IEnumerable<BaseCard> selectedCards);
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                MainController.AddEvent(new ReturnToDeckEvent(card));
                MainController.AddEvent(new DrawCardEvent(SourcePlayer));
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                MainController.AddEvent(new DamageCreatureEvent(card, Value));
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                MainController.AddEvent(new FortifyCreatureEvent(card, Value));
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                MainController.AddEvent(new WeakenCreatureEvent(card, Value));
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                MainController.AddEvent(new StrengthenCreatureEvent(card, Value));
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

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards.Cast<CreatureCard>())
            {
                MainController.AddEvent(new StrengthenCreatureEvent(card, Attack));
                MainController.AddEvent(new FortifyCreatureEvent(card, Defence));
            }
        }
    }

    public class DestroyTargetsEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => $"Destroy {Count} creatures";

        public DestroyTargetsEvent(TargetConditions targetConditions, int count) : base(targetConditions, count, SelectionType.Negative)
        {
        }

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                MainController.AddEvent(new DestroyCardEvent(card));
            }
        }
    }

    public class EliminateTargetsEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => $"Eliminate {Count} cards";

        public EliminateTargetsEvent(TargetConditions targetConditions, int count) : base(targetConditions, count, SelectionType.Negative)
        {
        }

        public override void FinishedSelection(IEnumerable<BaseCard> selectedCards)
        {
            foreach (var card in selectedCards)
            {
                MainController.AddEvent(new EliminateCardEvent(card));
            }
        }
    }

    public class CustomMultiTargetEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => Message;
        private string Message { get; set; }
        private Action<IEnumerable<BaseCard>> Action { get; set; }

        public CustomMultiTargetEvent(TargetConditions targetConditions, int count, SelectionType selectionType, string message, Action<IEnumerable<BaseCard>> action)
            : base(targetConditions, count, selectionType)
        {
            Message = message;
            Action = action;
        }

        public override void FinishedSelection(IEnumerable<BaseCard> targets) 
        {
            Action.Invoke(targets);
        }
    }

    public class CustomSingleTargetEvent : BaseCardSelectionEvent
    {
        public override string EventTitle => Message;
        private string Message { get; set; }
        private Action<BaseCard> Action { get; set; }

        public CustomSingleTargetEvent(TargetConditions targetConditions, SelectionType selectionType, string message, Action<BaseCard> action)
            : base(targetConditions, 1, selectionType)
        {
            Message = message;
            Action = action;
        }

        public override void FinishedSelection(IEnumerable<BaseCard> targets)
        {
            if (targets.Count() < 1)
                return;

            var target = targets.Single();
            Action.Invoke(target);
        }
    };
}
