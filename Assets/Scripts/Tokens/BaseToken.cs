using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tokens
{
    public class BaseToken : BaseEffectSource, ITargetable
    {
        public TokenType TokenType => (TokenType)Id;
        public string Name { get; set; }
        public string EffectText { get; set; }
        public string FlavourText { get; set; }

        public string Symbol { get; set; }

        protected BaseToken(TokenInfo tokenInfo)
        {
            Id = tokenInfo.TokenData.Id;
            Name = tokenInfo.TokenData.Name;
            EffectText = tokenInfo.TokenData.EffectText;
            FlavourText = tokenInfo.TokenData.FlavourText;
            Symbol = tokenInfo.TokenData.Symbol;

            Effect = tokenInfo.Effect;
        }

        // Factory method to create
        public static BaseToken Create(TokenInfo tokenInfo)
        {
            return new BaseToken(tokenInfo);
        }
    }

    public class SurgeToken : BaseToken
    {
        public SurgeToken(TokenInfo tokenInfo) : base(tokenInfo)
        {
        }
    }
}
