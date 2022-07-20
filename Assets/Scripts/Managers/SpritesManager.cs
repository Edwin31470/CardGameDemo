using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    // Simplifies getting sprites
    public static class SpritesManager
    {
        public static Sprite GetCostSymbol(int cost)
        {
            return Resources.Load<Sprite>($"Sprites/Symbols/Cost{cost}");
        }

        public static Sprite GetCardSymbol(string symbol)
        {
            return Resources.Load<Sprite>($"Sprites/Symbols/{symbol}Symbol");
        }

        public static Sprite GetLargeCard(Colour color)
        {
            return Resources.Load<Sprite>($"Sprites/{color}CardLarge");
        }

        public static Sprite GetSmallCard(Colour color, CardType cardType)
        {
            return Resources.Load<Sprite>($"Sprites/{color}CardSmall{cardType}");
        }
    }
}
