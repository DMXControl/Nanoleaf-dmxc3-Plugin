using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentEffectSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentEffectSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CurrentEffectSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnEffectEvent += ExternalControlEndpoint_StaticOnEffectEvent;
            SerialNumber = serialNumber;
            CurrentValue = NanoleafPlugin.getClient(SerialNumber).SelectedEffect;
        }

        private void CurrentEffectSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        private void ExternalControlEndpoint_StaticOnEffectEvent(object sender, EffectEventArgs e)
        {
            if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                return;

            EffectEvents events = e.EffectEvents;
            if (events == null)
                return;

            this.CurrentValue = events.Events.First().Value;
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentEffect", serialNumber);
        }
        private static string getDisplayName()
        {
            return "CurrentEffect";
        }
        private static ParameterCategory getCategory(string serialNumber)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber);
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.String; }
        }

        public override object Min => null;

        public override object Max => null;
    }
}
