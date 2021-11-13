using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class UnityExtensions
    {
        public static Color Normalise(this Color color)
        {
            return new Color(color.r / 255, color.g / 255, color.b / 255);
        }
    }
}
