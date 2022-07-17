using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class AllPanelInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("serialNo")]
        public string SerialNumber { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        [JsonProperty("hardwareVersion")]
        public string HardwareVersion { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("effects")]
        public Effects Effects { get; set; }

        [JsonProperty("panelLayout")]
        public PanelLayout PanelLayout { get; set; }

        [JsonProperty("state")]
        public States State { get; set; }
    }
}
