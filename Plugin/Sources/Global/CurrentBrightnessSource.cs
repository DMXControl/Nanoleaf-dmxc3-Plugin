using LumosLIB.Kernel;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentBrightnessSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentBrightnessSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), new ParameterCategory("Nanoleaf", getCategory(serialNumber)))
        {
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            var controler = NanoleafPlugin.getClient(SerialNumber);
            min = controler.BrightnessMin;
            max = controler.BrightnessMax;
            CurrentValue = controler.Brightness;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, EventArgs e)
        {
            StateEvents events = sender as StateEvents;
            if (events == null)
                return;

            var value = events.Events.FirstOrDefault(v => v.Attribute == StateEvent.EAttribute.Brightness);

            if (value != null)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentBrightness", serialNumber);
        }
        private static string getDisplayName()
        {
            return "Brightness";
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
