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
    public class CurrentPowerstateSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public CurrentPowerstateSource(string serialNumber) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CurrentPowerstateSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnStateEvent += ExternalControlEndpoint_StaticOnStateEvent;
            SerialNumber = serialNumber;
            CurrentValue = NanoleafPlugin.getClient(SerialNumber).PowerOn;
        }

        private void CurrentPowerstateSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        private void ExternalControlEndpoint_StaticOnStateEvent(object sender, StateEventArgs e)
        {
            try
            {
                if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                    return;

                if (!e.StateEvents.Events.Any(v => v.Attribute == StateEvent.EAttribute.On))
                    return;

                var value = e.StateEvents.Events.FirstOrDefault(v => v.Attribute == StateEvent.EAttribute.On);
                this.CurrentValue = value.Value;
            }
            catch (Exception ex)
            {
                NanoleafPlugin.Log.ErrorOrDebug(ex);
            }
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
