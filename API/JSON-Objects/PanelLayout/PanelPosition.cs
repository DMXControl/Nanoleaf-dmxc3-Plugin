using Newtonsoft.Json;

namespace Nanoleaf_Plugin.API
{
    public class PanelPosition
    {
        public enum EShapeType
        {
            Triangle = 0,
            Rhythm = 1,
            Square = 2,
            ControlSquarePrimary = 3,
            ContolSquarePassive = 4,
            PowerSupply = 5
        }
        [JsonProperty("panelId")]
        public int PanelId { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        [JsonProperty("o")]
        public int Orientation { get; set; }

        [JsonProperty("shapeType")]
        public EShapeType ShapeType { get; set; }

        public override string ToString()
        {
            return $"PanelID: {PanelId} X: {X} Y: {Y} Orentation: {Orientation} Shape: {ShapeType}";
        }
    }
}
