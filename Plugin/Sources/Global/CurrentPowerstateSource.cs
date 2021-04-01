using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentPowerstateSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentPowerstateSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber))
        {
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            CurrentValue = NanoleafPlugin.getClient(SerialNumber).PowerOn;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, StateEventArgs e)
        {
            if (!NanoleafPlugin.getClient(this.SerialNumber).IP.Equals(e.IP))
                return;
            StateEvents events = e.StateEvents;
            if (events == null)
                return;

            var value = events.Events.FirstOrDefault(v => v.Attribute == StateEvent.EAttribute.On);

            if (value != null)
                this.CurrentValue = value.Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentPowerstate", serialNumber);
        }
        private static string getDisplayName()
        {
            return "Powerstate";
        }
        private static ParameterCategory getCategory(string serialNumber)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber);
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.Bool; }
        }
        public override object Min => false;

        public override object Max => true;
    }
}
