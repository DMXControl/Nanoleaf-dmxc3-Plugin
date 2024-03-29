﻿using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CurrentHueSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentHueSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CurrentHueSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            var controller = NanoleafPlugin.getClient(SerialNumber);
            min = controller.HueMin;
            max = controller.HueMax;
            CurrentValue = controller.Hue;
        }

        private void CurrentHueSource_EnabledChanged(object sender, EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, StateEventArgs e)
        {
            try
            {
                if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                    return;

                if (!e.StateEvents.Events.Any(v => v.Attribute == StateEvent.EAttribute.Hue))
                    return;

                StateEvents events = e.StateEvents;
                var value = events.Events.First(v => v.Attribute == StateEvent.EAttribute.Hue);
                this.CurrentValue = value.Value;
            }
            catch (Exception ex)
            {
                NanoleafPlugin.Log.ErrorOrDebug(ex);
            }
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-CurrentHue", serialNumber);
        }
        private static string getDisplayName()
        {
            return "Hue";
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
