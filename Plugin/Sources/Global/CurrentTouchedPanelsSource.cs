using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;

namespace Nanoleaf_Plugin
{
    public class CurrentTouchedPanelsSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentTouchedPanelsSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnTouchEvent += ExternalControlEndpoint_StaticOnTouchEvent;
            SerialNumber = serialNumber;
            min = 0;
            max = ushort.MaxValue;
        }

        private void ExternalControlEndpoint_StaticOnTouchEvent(object sender, EventArgs e)
        {
            TouchEvent events = sender as TouchEvent;
            if (events == null)
                return;

            CurrentValue = events.TouchedPanelsNumber;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentTouchedPanels", serialNumber);
        }
        private static string getDisplayName()
        {
            return "TouchedPanels";
        }
        private static ParameterCategory getCategory(string serialNumber)
        {
            return new ParameterCategory(serialNumber);
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.NUMERIC; }
        }
        private object min;
        private object max;
        public override object Min => min;

        public override object Max => max;
    }
}
