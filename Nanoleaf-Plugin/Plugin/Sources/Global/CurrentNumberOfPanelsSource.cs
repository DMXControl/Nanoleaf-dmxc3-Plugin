using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentNumberOfPanelsSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentNumberOfPanelsSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnLayoutEvent += ExternalControlEndpoint_StaticOnLayoutEvent;
            SerialNumber = serialNumber;
            var numberOfPanels = NanoleafPlugin.getClient(SerialNumber).NumberOfPanels;
            min =0;
            max = int.MaxValue;
            CurrentValue = numberOfPanels;
        }

        private void ExternalControlEndpoint_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            if (!NanoleafPlugin.getClient(this.SerialNumber).IP.Equals(e.IP))
                return;

            LayoutEvent events = e.LayoutEvent;
            if (events == null)
                return;

            var value = events.Layout?.NumberOfPanels;

            if (value.HasValue)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentNumberOfPanels", serialNumber);
        }
        private static string getDisplayName()
        {
            return "NumberOfPanels";
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
