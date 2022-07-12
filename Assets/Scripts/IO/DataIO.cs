using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IO
{
    public static class DataIO
    {
        private static readonly JsonSerializer JsonSerializer = new();

        public static IEnumerable<T> ReadAll<T>() where T : BaseData
        {
            var folder = GetFolder(typeof(T));

            foreach (var filePath in Directory.EnumerateFiles(folder, "*.json"))
            {
                using var jsonReader = new JsonTextReader(new StreamReader(filePath));

                yield return JsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        public static T Read<T>(int id) where T : BaseData
        {
            var folder = GetFolder(typeof(T));

            using var jsonReader = new JsonTextReader(new StreamReader(folder + $"/{id}.json"));

            return JsonSerializer.Deserialize<T>(jsonReader);
        }

        public static void Write<T>(T data) where T : BaseData
        {
            var folder = GetFolder(typeof(T));

            using var jsonWriter = new JsonTextWriter(new StreamWriter(folder + $"/{data.Id}.json"));

            JsonSerializer.Serialize(jsonWriter, data);
        }

        private static string GetFolder(Type dataType)
        {
            if (dataType == typeof(CardData))
            {
                return "Assets/Data/Cards";
            }
            else if (dataType == typeof(ItemData))
            {
                return "Assets/Data/Items";
            }
            else if (dataType == typeof(TerrainData))
            {
                return "Assets/Data/Terrains";
            }

            throw new ArgumentOutOfRangeException(nameof(dataType), $"Type must be {nameof(CardData)} or {nameof(ItemData)}");
        }
    }
}
