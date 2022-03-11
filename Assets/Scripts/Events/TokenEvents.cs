using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    public class BaseTokenEvent : BasePlayerEvent
    {
        public BaseTokenEvent(PlayerType playerType) : base(playerType)
        {
        }
    }

    public class AddTokensEvent : BaseTokenEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayerType} Player gets {Amount} {TokenType} Tokens";

        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public AddTokensEvent(PlayerType playerType, TokenType tokenType, int amount) : base(playerType)
        {
            PlayerType = playerType;
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = getPlayer(PlayerType);
            player.AddTokens(TokenType, Amount);
            yield break;
        }
    }

    public class RemoveTokensEvent : BaseTokenEvent
    {
        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public RemoveTokensEvent(PlayerType playerType, TokenType tokenType, int amount) : base(playerType)
        {
            PlayerType = playerType;
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(Func<PlayerType, Player> getPlayer)
        {
            var player = getPlayer(PlayerType);
            player.RemoveTokens(TokenType, Amount);
            yield break;
        }
    }
}
