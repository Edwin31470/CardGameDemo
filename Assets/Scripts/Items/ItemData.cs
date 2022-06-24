using Assets.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.Scripts.Items
{
    public class ItemData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("itemType")]
        public ItemType ItemType { get; set; }
        [JsonProperty("effectText")]
        public string EffectText { get; set; }
        [JsonProperty("flavourText")]
        public string FlavourText { get; set; }
        [JsonProperty("isLegendary")]
        public bool IsLegendary { get; set; }
    }
}
