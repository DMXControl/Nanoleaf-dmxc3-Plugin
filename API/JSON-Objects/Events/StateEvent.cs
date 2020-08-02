using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nanoleaf_Plugin.API
{
    public class StateEvent
    {
        [JsonProperty("attr")]
        public EAttribute Attribute { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }
        public enum EAttribute
        {
            UNKNOWN,
            On,
            Brightness,
            Hue,
            Saturation,
            CCT,
            ColorMode
        }
        public override string ToString()
        {
            return $"State: {Attribute}: {Value}";
        }
    }
    public class StateEvents
    {
        [JsonProperty("events")]
        public IEnumerable<StateEvent> Events { get; set; }
    }
}
