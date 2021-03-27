using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentSaturationSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentSaturationSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            var controller = NanoleafPlugin.getClient(SerialNumber);
            min = controller.SaturationMin;
            max = controller.SaturationMax;
            CurrentValue = controller.Saturation;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, EventArgs e)
        {
            StateEvents events = sender as StateEvents;
            if (events == null)
                return;

            var value = events.Events.FirstOrDefault(v => v.Attribute == StateEvent.EAttribute.Saturation);

            if (value != null)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentSaturation", serialNumber);
        }
        private static string getDisplayName()
        {
            return "Saturation";
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
