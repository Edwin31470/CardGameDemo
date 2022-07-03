using Assets.Scripts.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.IO
{
    public static class PlayerIO
    {
        private const string PlayerFolder = "Assets/Data/Players";
        private const string DeckFolder = "Assets/Data/Decks";
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public static PlayerData ReadPlayer(PlayerType player)
        {
            using var jsonReader = new JsonTextReader(new StreamReader($"{PlayerFolder}/{player}.json"));

            return JsonSerializer.Deserialize<PlayerData>(jsonReader);
        }

        public static void WritePlayer(PlayerData playerData)
        {
            using var jsonWriter = new JsonTextWriter(new StreamWriter($"{PlayerFolder}/{playerData.PlayerType}.json"));

            JsonSerializer.Serialize(jsonWriter, playerData);
        }

        public static List<string> ReadDeckNames()
        {
            return Directory
                .GetFiles(DeckFolder, "*.json")
                .Select(Path.GetFileName)
                .Select(x => x.Substring(0, x.Length - 5))
                .ToList();
        }

        public static List<int> ReadDeck(string deckName)
        {
            using var jsonReader = new JsonTextReader(new StreamReader($"{DeckFolder}/{deckName}.json"));

            return JsonSerializer.Deserialize<List<int>>(jsonReader);
        }

        public static void WriteDeck(string deckName, IEnumerable<int> cardIds)
        {
            using var jsonWriter = new JsonTextWriter(new StreamWriter($"{DeckFolder}/{deckName}.json"));

            JsonSerializer.Serialize(jsonWriter, cardIds);
        }
    }
}
