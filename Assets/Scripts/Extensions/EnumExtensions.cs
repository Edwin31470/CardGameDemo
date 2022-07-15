using Assets.Scripts.Enums;
using Assets.Scripts.Tokens;
using System;

namespace Assets.Scripts.Extensions
{
    public static class EnumExtensions
    {
        public static PlayerType Opposite(this PlayerType player)
        {
            return ~player & PlayerType.Either;
        }

        public static bool HasFlagNotNone(this Enum enumValue, Enum flag)
        {
            ulong enumVal = Convert.ToUInt64(enumValue);
            ulong flagVal = Convert.ToUInt64(flag);

            return (enumVal & flagVal) == flagVal;
        }

        // Basic tokens are tokens with no effects. They are not removed when they have no associated events, unlike other tokens
        public static bool IsBasicToken(this BaseToken token)
        {
            switch (token.TokenType)
            {
                case TokenType.Surge:
                case TokenType.Claw:
                case TokenType.Blunt:
                case TokenType.Shell:
                case TokenType.Cracked:
                    return true;
                default:
                    return false;
            }
        }
    }
}
