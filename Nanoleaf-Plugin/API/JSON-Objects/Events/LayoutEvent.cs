using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Nanoleaf_Plugin.API
{
    public class LayoutEvent
    {
        public Layout Layout { get; set; }
        public int? GlobalOrientation { get; set; }
    }
    public class LayoutEventConverter : JsonConverter<LayoutEvent>
    {
        public static LayoutEventConverter Instance { get; private set; } = new LayoutEventConverter();
        public override LayoutEvent ReadJson(JsonReader reader, Type objectType, LayoutEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                Layout layout = null;
                int? globalOrientation = null;
                JObject jObject = JObject.Load(reader);
                foreach (var obj in jObject.Root.Children().Single().First())
                {
                    switch ((int)obj["attr"])
                    {
                        case 1:
                            layout = JsonConvert.DeserializeObject<Layout>(obj["value"].ToString());
                            break;
                        case 2:
                            globalOrientation = (int)obj["value"];
                            break;
                    }
                }
                return new LayoutEvent() { Layout = layout, GlobalOrientation = globalOrientation };
            }
            catch (Exception)
            {

            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, LayoutEvent value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
