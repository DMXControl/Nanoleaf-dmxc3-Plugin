using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentNumberOfPanelsSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentNumberOfPanelsSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CurrentNumberOfPanelsSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnLayoutEvent += ExternalControlEndpoint_StaticOnLayoutEvent;
            SerialNumber = serialNumber;
            var numberOfPanels = NanoleafPlugin.getClient(SerialNumber).NumberOfPanels;
            min =0;
            max = int.MaxValue;
            CurrentValue = numberOfPanels;
        }

        private void CurrentNumberOfPanelsSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        private void ExternalControlEndpoint_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                return;

            LayoutEvent _event = e.LayoutEvents.Events.Last();
            if (_event.Layout == null)
                return;
            Layout layout = _event.Layout.Value;

            var value = layout.NumberOfPanels;
            this.CurrentValue = value;

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
