using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class StateInfo
    {
        [JsonProperty("value")]
        public ushort Value { get; set; }

        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}