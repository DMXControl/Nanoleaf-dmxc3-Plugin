using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class StateOnOff
    {
        [JsonProperty("value")]
        public bool On { get; set; }
    }
}
