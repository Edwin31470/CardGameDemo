using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Terrains
{
    public class BaseTerrain : BaseEffectSource, ITargetable
    {
        public string Name { get; set; }
        public string EffectText { get; set; }
        public string FlavourText { get; set; }

        public TerrainType TerrainType { get; set; }


        protected BaseTerrain(TerrainInfo terrainInfo)
        {
            Id = terrainInfo.TerrainData.Id;
            Name = terrainInfo.TerrainData.Name;
            EffectText = terrainInfo.TerrainData.EffectText;
            FlavourText = terrainInfo.TerrainData.FlavourText;
            TerrainType = terrainInfo.TerrainData.TerrainType;

            Effect = terrainInfo.Effect;
        }

        // Factory method to create
        public static BaseTerrain Create(TerrainInfo terrainInfo)
        {
            return new BaseTerrain(terrainInfo);
        }
    }
}
