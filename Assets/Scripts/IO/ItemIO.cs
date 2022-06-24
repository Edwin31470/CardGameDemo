using Assets.Scripts.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IO
{
    internal class ItemIO
    {
        private const string ItemFolder = "Assets/Data/Items";
        private static readonly JsonSerializer JsonSerializer = new();

        public static IEnumerable<ItemData> ReadAll()
        {
            foreach (var filePath in Directory.EnumerateFiles(ItemFolder, "*.json"))
            {
                using var jsonReader = new JsonTextReader(new StreamReader(filePath));

                yield return JsonSerializer.Deserialize<ItemData>(jsonReader);
            }
        }

        public static ItemData ReadItem(int id)
        {
            using var jsonReader = new JsonTextReader(new StreamReader(ItemFolder + $"/{id}.json"));

            return JsonSerializer.Deserialize<ItemData>(jsonReader);
        }

        public static void WriteItem(ItemData data)
        {
            using var jsonWriter = new JsonTextWriter(new StreamWriter(ItemFolder + $"/{data.Id}.json"));

            JsonSerializer.Serialize(jsonWriter, data);
        }
    }
}
