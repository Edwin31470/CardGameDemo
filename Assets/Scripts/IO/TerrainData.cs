using Assets.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class TerrainData : BaseData
    {
        [JsonProperty("terrainType")]
        public TerrainType TerrainType { get; set; }
    }
}
