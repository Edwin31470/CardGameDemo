using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    // Events that affect the player's stats

    public class BasePlayerEvent : BaseBoardEvent
    {
        protected PlayerType PlayerType { get; set; }

        public BasePlayerEvent(PlayerType playerType)
        {
            PlayerType = playerType;
        }
    }

    public class DamagePlayerEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayerType} Player takes {Amount} damage";

        private int Amount { get; set; }

        public DamagePlayerEvent(PlayerType playerType, int amount) : base(playerType)
        {
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);
            player.Health.Remove(Amount);

            if (player.Health.Get() <= 0)
                yield return new GameEndEvent($"{player.PlayerType} Player has run out of life!");
        }
    }

    public class AddLifePlayerEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayerType} Player heals for {Amount}";

        private int Amount { get; set; }

        public AddLifePlayerEvent(PlayerType playerType, int amount) : base(playerType)
        {
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);
            player.Health.Add(Amount);
            yield break;
        }
    }

    public class AddManaPlayerEvent : BasePlayerEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayerType} Player gains {Amount} {Colour} mana";

        private Colour Colour { get; set; }
        private int Amount { get; set; }

        public AddManaPlayerEvent(PlayerType playerType, Colour colour, int amount) : base(playerType)
        {
            Colour = colour;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            var player = board.GetPlayer(PlayerType);
            player.AddMana(Colour, Amount);
            yield break;
        }
    }
}
