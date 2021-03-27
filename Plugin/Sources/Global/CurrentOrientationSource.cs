using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentOrientationSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentOrientationSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnLayoutEvent += ExternalControlEndpoint_StaticOnLayoutEvent;
            SerialNumber = serialNumber;
            var controller = NanoleafPlugin.getClient(SerialNumber);
            min = controller.GlobalOrientationMin;
            max = controller.GlobalOrientationMax;
            CurrentValue = controller.GlobalOrientation;
        }

        private void ExternalControlEndpoint_StaticOnLayoutEvent(object sender, EventArgs e)
        {
            LayoutEvent events = sender as LayoutEvent;
            if (events == null)
                return;

            var value = events.GlobalOrientation;

            if (value.HasValue)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentOrientation", serialNumber);
        }
        private static string getDisplayName()
        {
            return "Orientation";
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
