using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public abstract class BaseData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("effectText")]
        public string EffectText { get; set; }
        [JsonProperty("flavourText")]
        public string FlavourText { get; set; }
    }
}
