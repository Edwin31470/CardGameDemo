using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LegendkeeperIntegration
{
    public class Page
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("tags")]
        public string[] Tags { get; set; }
        [JsonProperty("documents")]
        public Document[] Documents { get; set; }
        public string MainContent => Documents.First(x => x.Name == "Main").Content;
    }
}
