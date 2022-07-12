using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using Assets.Scripts.Cards;

namespace Assets.Scripts.Events
{
    // Events that affect the player's stats. The player being affected is the source of the event.

    public class BasePlayerEvent : BaseSourceEvent<Player>
    {
        public BasePlayerEvent(Player player) : base(player)
        {
        }
    }

    public class DamagePlayerEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player takes {Value} damage";

        public int Value { get; set; }

        public DamagePlayerEvent(Player player, int value) : base(player)
        {
            Value = value;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.Health.Remove(Value);

            if (Source.Health.Get() <= 0)
                yield return new GameEndEvent($"{Source.PlayerType} Player has run out of life!");
        }
    }

    public class AddLifePlayerEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player heals for {Amount}";

        private int Amount { get; set; }

        public AddLifePlayerEvent(Player player, int amount) : base(player)
        {
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.Health.Add(Amount);
            yield break;
        }
    }

    public class AddManaEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player gains {Amount} {Colour} mana";

        private Colour Colour { get; set; }
        private int Amount { get; set; }

        public AddManaEvent(Player player, Colour colour, int amount) : base(player)
        {
            Colour = colour;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.AddMana(Colour, Amount);
            yield break;
        }
    }

    public class ManaSacrificeEvent : BasePlayerEvent
    {
        private BaseCard Card { get; set; }

        public ManaSacrificeEvent(Player player, BaseCard card) : base(player)
        {
            Card = card;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.RemoveFromHand(Card);
            Source.AddToEliminated(Card);

            yield return new SacrificeCardUIEvent(Card);
        }
    }

    public class EmptyDestroyPileEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Emptying Destroyed pile for {Source.PlayerType} Player";

        public EmptyDestroyPileEvent(Player player) : base(player)
        {
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            foreach (var card in Source.Destroyed)
            {
                yield return new EliminateCardEvent<BaseCard>(card, false);
            }
        }
    }

    public class GainControlOfFieldCardEvent : BasePlayerEvent
    {
        private FieldCard Target { get; set; }

        public GainControlOfFieldCardEvent(Player player, FieldCard target) : base(player)
        {
            Target = target;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var targetOwner = board.GetSourceOwner(Target);

            // If the player already owns the target, do nothing
            if (targetOwner == Source)
                yield break;

            targetOwner.RemoveFromField(Target);

            var slot = Source.GetRandomEmptySlot();

            if (slot != null)
            {
                slot.AddCard(Target);
                yield return new MoveCardToFieldUIEvent(Target, slot);
            }
            else // If no valid slot to move to, destroy the card instead
            {
                yield return new DestroyCardUIEvent(Source.PlayerType, Target);
            }
        }
    }

    public class SummonCardEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"Summoning {CardInfo.CardData.Name}";

        private CardInfo CardInfo { get; set; }

        public SummonCardEvent(Player player, CardInfo cardInfo) : base(player)
        {
            cardInfo.IsSummoned = true;
            CardInfo = cardInfo;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var summonedCard = BaseCard.Create(CardInfo);

            if (summonedCard is ActionCard actionCard)
            {
                yield return new PlayActionCardEvent(actionCard);
            }
            else if (summonedCard is FieldCard fieldCard)
            {
                var slot = Source.GetRandomEmptySlot();
                if (slot == null)
                {
                    yield return new MessageEvent("Unable to summon card: no free slots", 1);
                    yield break;
                }

                yield return new CreateCardInSlotUIEvent(Source.PlayerType, summonedCard, slot);
                yield return new EnterFieldEvent<FieldCard>(fieldCard, slot);
            }
        }
    }
}
