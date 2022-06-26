using Assets.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class ItemData : BaseData
    {
        [JsonProperty("itemType")]
        public ItemType ItemType { get; set; }
        [JsonProperty("isLegendary")]
        public bool IsLegendary { get; set; }
    }
}
