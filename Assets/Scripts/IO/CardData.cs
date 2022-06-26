using Assets.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class CardData : BaseData
    {
        [JsonProperty("colour")]
        public Colour Colour { get; set; }
        [JsonProperty("cost")]
        public int Cost { get; set; }
        [JsonProperty("cardType")]
        public CardType CardType { get; set; }
        [JsonProperty("subTypes")]
        public SubType SubTypes { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("attack")]
        public int Attack { get; set; }
        [JsonProperty("defence")]
        public int Defence { get; set; }
        [JsonProperty("hasPersistence")]
        public bool HasPersistence { get; set; }
        [JsonProperty("isUnique")]
        public bool IsUnique { get; set; }
    }
}