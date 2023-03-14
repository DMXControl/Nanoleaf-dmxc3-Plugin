using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.PropertyType;
using System;
using System.Drawing;

namespace Nanoleaf_Plugin
{
    public class CanvasSink : AbstractInputSink
    {
        public readonly Panel Panel;
        public readonly string SerialNumber;

        public CanvasSink(string serialNumber, Panel panel, bool usePanelID = false) :
            base(getID(serialNumber, panel, usePanelID), getDisplayName(panel, usePanelID), getCategory(serialNumber, panel, usePanelID))

        {
            SerialNumber = serialNumber;
            Panel = panel;
        }

        private static string getID(string serialNumber, Panel panel, bool usePanelID)
        {
            if (usePanelID)
                return string.Format("Nanoleaf-{0}-PanelID:{1}", serialNumber, panel.ID);

            double x = panel.X / panel.SideLength;
            double y = panel.Y / panel.SideLength;
            return string.Format("Nanoleaf-{0}-Coordinates:{1}:{2}", serialNumber, x, y);
        }
        private static string getDisplayName(Panel panel, bool usePanelID)
        {
            if (usePanelID)
                return string.Format("Canvas {0}", panel.ID);

            double x = panel.X / panel.SideLength;
            double y = panel.Y / panel.SideLength;
            return string.Format("Canvas {0}:{1}", x, y);
        }
        private static ParameterCategory getCategory(string serialNumber, Panel panel, bool usePanelID)
        {
            if (usePanelID)
                return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber, "PanelIDs");
            else
                return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber, "Coordinates");
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.Color; }
        }

        public override object Min => Color.White;

        public override object Max => Color.Black;

        public override bool UpdateValue(object newValue)
        {
            if (!NanoleafMainSwitch.getInstance().Enabled) return true;

            RGBW rgbw = new RGBW();
            if (newValue is Color)
            {
                var color = (Color)newValue;
                rgbw = new RGBW(color.R, color.G, color.B);
            }
            else if (newValue is LumosColor)
            {
                var lumosColor = (LumosColor)newValue;
                rgbw = new RGBW((byte)(lumosColor.Red * 255), (byte)(lumosColor.Green * 255), (byte)(lumosColor.Blue * 255));
            }
            else if (newValue is RGBW)
            {
                rgbw = (RGBW)newValue;
            }
            else return false;
            try
            {
                var Controler = NanoleafPlugin.getClient(SerialNumber);                
                return Controler.SetPanelColor(Panel.ID, rgbw);
            }
            catch (Exception e)
            {
                NanoleafPlugin.Log.Error(string.Empty, e);
            }
            return false;
        }
    }
}
