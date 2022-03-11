using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public class AddTokensEvent : BaseGameplayEvent
    {
        //public override float Delay => 1f;
        //public override string EventTitle => $"{PlayerType} Player gets {Amount} {TokenType} Tokens";

        private PlayerType PlayerType { get; set; }
        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public AddTokensEvent(PlayerType playerType, TokenType tokenType, int amount)
        {
            PlayerType = playerType;
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(PlayerType);
            player.AddTokens(TokenType, Amount);
            yield break;
        }
    }

    public class RemoveTokensEvent : BaseGameplayEvent
    {
        private PlayerType PlayerType { get; set; }
        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public RemoveTokensEvent(PlayerType playerType, TokenType tokenType, int amount)
        {
            PlayerType = playerType;
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process()
        {
            var player = MainController.GetPlayer(PlayerType);
            player.RemoveTokens(TokenType, Amount);
            yield break;
        }
    }
}
