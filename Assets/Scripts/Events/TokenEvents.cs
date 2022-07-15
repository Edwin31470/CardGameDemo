using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using Assets.Scripts.Tokens;
using System.Linq;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Events
{
    // An event that alters token piles (NOT an event used by tokens)
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
            var events = new List<BaseEvent>();
            for (var i = 0; i < Amount; i++)
            {
                events.AddRange(Source.AddToken(TokenType).GetEvents(board));
            }

            return events;
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
            for (var i = 0; i < Amount; i++)
            {
                Source.RemoveToken(TokenType);
            }            
            yield break;
        }
    }
}
