using Assets.Scripts.Enums;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LegendkeeperIntegration
{
    public static class Extensions
    {
        public static string ParseStringField(this HtmlNodeCollection collection, string fieldName)
        {
            return collection.FirstOrDefault(x => x.InnerText == fieldName)?.NextSibling?.InnerText;
        }

        public static int ParseIntField(this HtmlNodeCollection collection, string fieldName)
        {
            return int.Parse(collection.ParseStringField(fieldName) ?? "0");
        }
        public static T ParseEnumField<T>(this HtmlNodeCollection collection, string fieldName) where T : struct, Enum
        {
            var value = collection.ParseStringField(fieldName);

            Enum.TryParse(value, true, out T parsedValue);

            return parsedValue;
        }

        public static IEnumerable<T> ParseMultipleEnumField<T>(this HtmlNodeCollection collection, string fieldName) where T : struct, Enum
        {
            var types = collection.ParseStringField(fieldName).Replace(" ", "").Split(',');

            if (types.Count() == 1 && types[0] == "")
            {
                Enum.TryParse("None", true, out T none);
                yield return none;
                yield break;
            }

            foreach (var type in types)
            {
                Enum.TryParse(type, true, out T parsedValue);
                if (parsedValue.ToString() == "None")
                    throw new ArgumentOutOfRangeException("parsedValue", $"{type} could not be parsed: is something mispelled?");

                yield return parsedValue;
            }
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return typeof(T).GetEnumValues().OfType<T>();
        }
    }
}
