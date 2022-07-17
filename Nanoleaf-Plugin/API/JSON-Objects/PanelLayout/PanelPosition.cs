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
            PowerSupply = 5,
            Hexagon_Shapes = 7,
            Triangle_Shapes = 8,
            MiniTriangle_Shapes = 9,
            ShapesController = 12
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

        public int SideLength
        {
            get
            {
                switch (this.ShapeType)
                {
                    case EShapeType.Triangle:
                        return 150;
                    case EShapeType.Rhythm:
                    case EShapeType.ShapesController:
                        return 0;
                    case EShapeType.Square:
                    case EShapeType.ContolSquarePassive:
                    case EShapeType.ControlSquarePrimary:
                        return 100;
                    case EShapeType.Hexagon_Shapes:
                    case EShapeType.MiniTriangle_Shapes:
                        return 67;
                    case EShapeType.Triangle_Shapes:
                        return 134;
                }
                return 0;
            }
        }
    }
}
