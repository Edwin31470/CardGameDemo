using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extensions
{
    public static class MathExtensions
    {
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        public static bool IsBetween(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
}
