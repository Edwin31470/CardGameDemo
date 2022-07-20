using Assets.Scripts.Effects;
using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using Assets.Scripts.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Managers
{
    public static class TokensManager
    {
        private static readonly Dictionary<TokenType, TokenData> TokenLibrary = DataIO.ReadAll<TokenData>().ToDictionary(x => (TokenType)x.Id, x => x);

        private static readonly Dictionary<TokenType, BaseEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseEffect))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseSourceEffect<BaseToken>)))
            .Select(x => (BaseEffect)Activator.CreateInstance(x))
            .ToDictionary(x => (TokenType)x.Id, x => x);

        public static BaseToken GetToken(TokenType tokenType)
        {
            return BaseToken.Create(new TokenInfo
            {
                TokenData = TokenLibrary[tokenType],
                Effect = EffectLibrary.GetValueOrDefault(tokenType)
            });
        }
    }
}
