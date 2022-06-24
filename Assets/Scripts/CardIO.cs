using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Cards;
using Newtonsoft.Json;

namespace Assets.Scripts
{
    public static class CardIO
    {
        private const string CardFolder = "Assets/Data/Cards";
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public static IEnumerable<CardData> ReadAll()
        {
            foreach (var filePath in Directory.EnumerateFiles(CardFolder, "*.json"))
            {
                using (var jsonReader = new JsonTextReader(new StreamReader(filePath)))
                {
                    yield return JsonSerializer.Deserialize<CardData>(jsonReader);
                }
            }
        }

        public static CardData ReadCard(int id)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(CardFolder + $"/{id}.json")))
            {
                return JsonSerializer.Deserialize<CardData>(jsonReader);
            }
        }

        public static void WriteCard(CardData data)
        {
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(CardFolder + $"/{data.Id}.json")))
            {
                JsonSerializer.Serialize(jsonWriter, data);
            }
        }
    }
}
