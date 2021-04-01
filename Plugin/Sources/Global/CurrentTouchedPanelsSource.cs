using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;

namespace Nanoleaf_Plugin
{
    public class CurrentTouchedPanelsSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentTouchedPanelsSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber))
        {
            Communication.StaticOnTouchEvent += ExternalControlEndpoint_StaticOnTouchEvent;
            SerialNumber = serialNumber;
            min = 0;
            max = ushort.MaxValue;
        }

        private void ExternalControlEndpoint_StaticOnTouchEvent(object sender, TouchEventArgs e)
        {
            if (!NanoleafPlugin.getClient(this.SerialNumber).IP.Equals(e.IP))
                return;

            TouchEvent events = e.TouchEvent;
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
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber);
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.Numeric; }
        }
        private object min;
        private object max;
        public override object Min => min;

        public override object Max => max;
    }
}
