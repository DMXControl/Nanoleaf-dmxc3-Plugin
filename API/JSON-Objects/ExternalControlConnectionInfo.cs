using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class ExternalControlConnectionInfo
    {
        [JsonProperty("streamControlIpAddr")]
        public string StreamIPAddress { get; set; }

        [JsonProperty("streamControlPort")]
        public int StreamPort { get; set; }

        [JsonProperty("streamControlProtocol")]
        public string StreamProtocol { get; set; }
    }
}
