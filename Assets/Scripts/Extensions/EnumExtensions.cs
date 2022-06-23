using Assets.Scripts.Enums;
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
    }
}
