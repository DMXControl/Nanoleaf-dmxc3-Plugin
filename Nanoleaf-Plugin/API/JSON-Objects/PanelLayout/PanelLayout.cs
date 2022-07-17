using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class PanelLayout
    {
        [JsonProperty("globalOrientation")]
        public StateInfo GlobalOrientation { get; set; }

        [JsonProperty("layout")]
        public Layout Layout { get; set; }
    }
}
