using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class States
    {
        [JsonProperty("on")]
        public StateOnOff On { get; set; }
        [JsonProperty("brightness")]
        public StateInfo Brightness { get; set; }

        [JsonProperty("hue")]
        public StateInfo Hue { get; set; }

        [JsonProperty("sat")]
        public StateInfo Saturation { get; set; }

        [JsonProperty("ct")]
        public StateInfo ColorTemprature { get; set; }

        [JsonProperty("colorMode")]
        public string ColorMode { get; set; }
    }
}