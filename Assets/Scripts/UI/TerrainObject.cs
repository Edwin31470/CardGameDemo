using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Terrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TerrainObject : BaseUIObject
    {
        public BaseTerrain TerrainReference { get; set; }
        public override ITargetable SourceReference => TerrainReference;

        private SpriteRenderer Glow { get; set; }

        public void Initialize(BaseTerrain terrainReference)
        {
            TerrainReference = terrainReference;

            Glow = transform.Find("Glow").gameObject.GetComponent<SpriteRenderer>();
            Glow.color = GetGlowColour(terrainReference.TerrainType);
        }

        private Color GetGlowColour(TerrainType terrainType)
        {
            if (terrainType.HasFlag(TerrainType.Neutral))
            {
                return new Color(0.9f, 0.7f, 0.15f);
            }
            else if (terrainType.HasFlag(TerrainType.Positive))
            {
                return new Color(0.12f, 0.62f, 0.16f);
            }
            else if (terrainType.HasFlag(TerrainType.Negative))
            {
                return new Color(0.86f, 0.27f, 0.12f);
            }

            throw new ArgumentOutOfRangeException(nameof(terrainType), "Invalid selection type for colour");
        }
    }
}
