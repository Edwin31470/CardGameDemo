using Assets.Scripts.Effects;
using Assets.Scripts.Events;
using Assets.Scripts.IO;
using Assets.Scripts.Items;
using Assets.Scripts.Terrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Managers
{
    public static class TerrainManager
    {
        private static readonly Dictionary<int, TerrainData> TerrainLibary = DataIO.ReadAll<TerrainData>().ToDictionary(x => x.Id, x => x);

        private static readonly Dictionary<int, BaseEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseEffect))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseSourceEffect<BaseTerrain>)))
            .Select(x => (BaseEffect)Activator.CreateInstance(x))
            .ToDictionary(x => x.Id, x => x);

        public static TerrainInfo GetTerrain(int id)
        {
            return new TerrainInfo
            {
                TerrainData = TerrainLibary[id],
                Effect = EffectLibrary[id]
            };
        }

        public static TerrainInfo GetTerrain(string terrainName)
        {
            var terrainData = TerrainLibary.Single(x => x.Value.Name == terrainName).Value;

            return new TerrainInfo
            {
                TerrainData = terrainData,
                Effect = EffectLibrary[terrainData.Id]
            };
        }
    }
}
