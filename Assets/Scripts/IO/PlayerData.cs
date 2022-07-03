using Assets.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class PlayerData
    {
        [JsonProperty("playerType")]
        public PlayerType PlayerType { get; set; }
        [JsonProperty("name")]
        public string DeckName { get; set; }
        [JsonProperty("weaponId")]
        public int WeaponId { get; set; }
        [JsonProperty("armourId")]
        public int ArmourId { get; set; }
        [JsonProperty("accessoryId")]
        public int AccessoryId { get; set; }
    }
}
