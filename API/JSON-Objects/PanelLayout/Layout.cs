using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class Layout
    {
        [JsonProperty("numPanels")]
        public uint NumberOfPanels { get; set; }

        [JsonProperty("sideLength")]
        public int SideLength { get; set; }

        [JsonProperty("positionData")]
        public IEnumerable<PanelPosition> PanelPositions { get; set; }
    }
}
