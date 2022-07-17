using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nanoleaf_Plugin.API
{
    public class Effects
    {
        [JsonProperty("effectsList")]
        public IEnumerable<string> List { get; set; }

        [JsonProperty("select")]
        public string Selected { get; set; }
    }
}
