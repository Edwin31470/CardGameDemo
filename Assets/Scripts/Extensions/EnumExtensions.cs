using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extensions
{
    public static class EnumExtensions
    {
        public static PlayerType GetOpposite(this PlayerType player)
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
