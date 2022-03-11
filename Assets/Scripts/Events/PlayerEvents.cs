using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public class DamagePlayerEvent : BaseGameplayEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayerType} Player takes {Amount} damage";

        private PlayerType PlayerType { get; set; }
        private int Amount { get; set; }

        public DamagePlayerEvent(PlayerType playerType, int amount)
        {
            PlayerType = playerType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(PlayerType);
            player.Health.Remove(Amount);

            if (player.Health.Get() <= 0)
                yield return new GameEndEvent();
        }
    }

    public class AddLifePlayerEvent : BaseGameplayEvent
    {
        //public override float Delay => 1f;
        //public override string EventTitle => $"{PlayerType} Player heals for {Amount}";

        private PlayerType Player { get; set; }
        private int Amount { get; set; }

        public AddLifePlayerEvent(PlayerType player, int amount)
        {
            Player = player;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(Player);
            player.Health.Add(Amount);
            yield break;
        }
    }

    public class AddManaPlayerEvent : BaseGameplayEvent
    {
        //public override float Delay => 1f;
        //public override string EventTitle => $"{PlayerType} Player heals for {Amount}";

        private PlayerType Player { get; set; }
        private Colour Colour { get; set; }
        private int Amount { get; set; }

        public AddManaPlayerEvent(PlayerType player, Colour colour, int amount)
        {
            Player = player;
            Colour = colour;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(Player);
            player.AddMana(Colour, Amount);
            yield break;
        }
    }
}
