using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.Events
{
    public abstract class BaseTokenEvent : BasePlayerEvent
    {
        public BaseTokenEvent(Player player) : base(player)
        {
        }
    }

    public class AddTokensEvent : BaseTokenEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player gets {Amount} {TokenType} Tokens";

        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public AddTokensEvent(Player player, TokenType tokenType, int amount) : base(player)
        {
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.AddTokens(TokenType, Amount);
            yield break;
        }
    }

    public class RemoveTokensEvent : BaseTokenEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player loses {Amount} {TokenType} Tokens";

        private TokenType TokenType { get; set; }
        private int Amount { get; set; }

        public RemoveTokensEvent(Player player, TokenType tokenType, int amount) : base(player)
        {
            TokenType = tokenType;
            Amount = amount;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Source.RemoveTokens(TokenType, Amount);
            yield break;
        }
    }
}
