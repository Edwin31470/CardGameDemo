using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class TokenData : BaseData
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
