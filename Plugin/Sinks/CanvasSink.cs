﻿using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.PropertyType;
using System;
using System.Drawing;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CanvasSink : AbstractInputSink
    {
        public readonly Panel Panel;
        public readonly string SerialNumber;

        public CanvasSink(string serialNumber, Panel panel, bool usePanelID = false) :
            base(getID(serialNumber, panel, usePanelID), getDisplayName(panel, usePanelID), new ParameterCategory("Nanoleaf", getCategory(serialNumber, panel, usePanelID)))

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
                return new ParameterCategory(serialNumber, new ParameterCategory("PanelIDs"));
            else
                return new ParameterCategory(serialNumber, new ParameterCategory("Coordinates"));
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.COLOR; }
        }

        public override object Min => Color.White;

        public override object Max => Color.Black;

        public override bool UpdateValue(object newValue)
        {
            Panel.RGBW rgbw = new Panel.RGBW();
            if (newValue is Color)
            {
                var color = (Color)newValue;
                rgbw = new Panel.RGBW(color.R, color.G, color.B);
            }
            else if (newValue is LumosColor)
            {
                var lumosColor = (LumosColor)newValue;
                rgbw = new Panel.RGBW((byte)(lumosColor.Red * 255), (byte)(lumosColor.Green * 255), (byte)(lumosColor.Blue * 255));
            }
            else if (newValue is Panel.RGBW)
            {
                rgbw = (Panel.RGBW)newValue;
            }
            else return false;
            try
            {
                var panel = NanoleafPlugin.getClient(SerialNumber).Panels.FirstOrDefault(p => p.ID.Equals(Panel.ID));
                if (panel != null)
                {
                    panel.StreamingColor = rgbw;
                    return true;
                }
            }
            catch (Exception e)
            {
                NanoleafPlugin.Log.Error(string.Empty, e);
            }
            return false;
        }
    }
}
