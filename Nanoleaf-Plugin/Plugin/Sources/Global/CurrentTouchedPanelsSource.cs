using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System;

namespace Nanoleaf_Plugin
{
    public class CurrentTouchedPanelsSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentTouchedPanelsSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CurrentTouchedPanelsSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnTouchEvent += ExternalControlEndpoint_StaticOnTouchEvent;
            SerialNumber = serialNumber;
            min = 0;
            max = ushort.MaxValue;
        }

        private void CurrentTouchedPanelsSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        private void ExternalControlEndpoint_StaticOnTouchEvent(object sender, TouchEventArgs e)
        {
            try
            {
                if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                    return;

                CurrentValue = e.TouchEvent.TouchedPanelsNumber;
            }
            catch (Exception ex)
            {
                NanoleafPlugin.Log.ErrorOrDebug(ex);
            }
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
