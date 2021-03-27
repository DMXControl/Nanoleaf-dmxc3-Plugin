using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentCTSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentCTSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            var controller = NanoleafPlugin.getClient(SerialNumber);
            min = controller.ColorTempratureMin;
            max = controller.ColorTempratureMax;
            CurrentValue = controller.ColorTemprature;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, StateEventArgs e)
        {
            if (!NanoleafPlugin.getClient(this.SerialNumber).IP.Equals(e.IP))
                return;
            StateEvents events = e.StateEvents;
            if (events == null)
                return;

            var value = events.Events.FirstOrDefault(v => v.Attribute == StateEvent.EAttribute.CCT);

            if (value != null)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentCT", serialNumber);
        }
        private static string getDisplayName()
        {
            return "CT";
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
