using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nanoleaf_Plugin.API
{
    public class EffectEvent
    {
        [JsonProperty("attr")]
        public int Attribute { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        public override string ToString()
        {
            return $"Effect: {Value}";
        }
    }
    public class EffectEvents
    {
        [JsonProperty("events")]
        public IEnumerable<EffectEvent> Events { get; set; }
    }
}
