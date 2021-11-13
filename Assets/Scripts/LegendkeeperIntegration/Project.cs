using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.LegendkeeperIntegration
{
    class Project
    {
        [JsonProperty("resources")]
        public Page[] Resources { get; set; }
    }
}
