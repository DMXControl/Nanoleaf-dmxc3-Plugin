using NanoleafAPI;
using org.dmxc.lumos.Kernel.DeviceProperties;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.HAL.Handler;
using org.dmxc.lumos.Kernel.PropertyType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.dmxc.lumos.Kernel.HAL.Handler.Matrix;
using LumosLIB.Tools;
using org.dmxc.lumos.Kernel.HAL.Handler.Helper;
using Nanoleaf_Plugin.Plugin.Device;
using Nanoleaf_Plugin.Plugin.MainSwitch;

namespace Nanoleaf_Plugin
{
    sealed class NanoleafControllerHandlerNode : AbstractHandlerNode
    {
        private string controllerId;
        private EDeviceType deviceType = EDeviceType.UNKNOWN;
        private Controller _instance = null;
        private double dimmerValue;
        public double DimmerValue
        {
            get { if (!this._strobeOff || this.StrobeValue == 0) return this.dimmerValue; else return 0; }
            set
            {
                this.dimmerValue = value;
                this.sendValueToController();
            }
        }
        public double StrobeValue
        {
            get { return this._strobeEmulator == null ? 1.0 : this._strobeEmulator.Frequency; }
            private set
            {
                if (this._strobeEmulator != null)
                    this._strobeEmulator.Frequency = value;

                this.sendValueToController();
            }
        }

        private DimmerProperty DimmerProperty
        {
            get;
            set;
        }
        private StrobeProperty StrobeProperty
        {
            get;
            set;
        }

        public override IDeviceBeam ParentBeam
        {
            get { return base.ParentBeam; }
            set
            {
                if (value != null && value.ParentDevice.Type.Equals(NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME))
                {
                    //Setting base.ParentBeam throws an Exception when Beam has already been set.
                    base.ParentBeam = value;
                    ((NanoleafControllerDevice)value.ParentDevice).ControllerIDChanged += new EventHandler(NanoleafHandlerNode_ControllerIDChanged);
                    controllerId = ((NanoleafControllerDevice)value.ParentDevice).SerialNumber;
                    deviceType = ((NanoleafControllerDevice)value.ParentDevice).DeviceType;
                    this._instance = NanoleafPlugin.getClient(controllerId);
                }
                else
                    throw new ArgumentException("This Type of Handler needs to be assigned to a " + NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME);
            }
        }

        private bool _strobeOff = true;
        private StrobeEmulator _strobeEmulator;
        private HALHandleContext _lastStrobeContext;
        private NanoleafControllerHandlerNode()
            : base()
        {
            NanoleafPlugin.ControllerAdded += NanoleafPlugin_ControllerAdded;
        }

        private void NanoleafPlugin_ControllerAdded(object sender, EventArgs e)
        {
            if (this._instance == null && !string.IsNullOrWhiteSpace(controllerId))
            {
                this.setInstance();
            }
        }

        protected override PropertyHandlerWorker getPropertyWorker(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return new PropertyHandlerWorker(handleDimmerValue);
            if (prop is StrobeProperty)
                return new PropertyHandlerWorker(handleStrobeValue);

            return null;
        }

        protected override List<PropertyDependencyBag> PropHandleProperties
        {
            get
            {
                List<PropertyDependencyBag> props = new List<PropertyDependencyBag>();
                //if (this.ColorProperty == null)
                //{
                //    this.ColorProperty = new ColorProperty(this.ParentBeam, hasRgb: true);
                //}
                //props.Add(new PropertyDependencyBag(this.ColorProperty, getDependencies()));
                return props;
            }
        }

        protected override List<PropertyDependencyBag> OptionalPropHandleProperties
        {
            get
            {
                if (this.DimmerProperty == null)
                {
                    this.DimmerProperty = new DimmerProperty(this.ParentBeam);
                }
                if (this.StrobeProperty == null)
                {
                    this.StrobeProperty = new StrobeProperty(this.ParentBeam);
                }

                List<PropertyDependencyBag> optProps = new List<PropertyDependencyBag>();
                optProps.Add(new PropertyDependencyBag(this.DimmerProperty, getDependencies(),
                    this.ParentHandler is AbstractDMXHandlerNode));

                optProps.Add(new PropertyDependencyBag(this.StrobeProperty, getDependencies(),
                    this.ParentHandler is AbstractDMXHandlerNode));

                return optProps;
            }
        }
        private void sendValueToController()
        {
            if (this._instance != null && NanoleafMainSwitch.getInstance().Enabled)
                _instance.Brightness = (ushort)(_strobeOff ? dimmerValue * 100 : 0);
        }
        protected override IPropertyType getPropTypeInstance(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return new IntensityType<double>(0.0, 100.0);
            if (prop is StrobeProperty)
                return new StrobeType(StrobeEmulator.EMULATOR_MIN, StrobeEmulator.EMULATOR_MAX);

            return null;
        }

        protected override IDeviceProperty SingleHandleProperty
        {
            get { return null; }
        }

        private void NanoleafHandlerNode_ControllerIDChanged(object sender, EventArgs e)
        {
            var d = sender as NanoleafControllerDevice;
            if (d != null)
            {
                this.deviceType = d.DeviceType;
                this.controllerId = d.SerialNumber;
                this.setInstance();
            }
        }
        private void setInstance()
        {
            this._instance = NanoleafPlugin.getClient(controllerId);
        }

        protected override void initializeHandler()
        {
            base.initializeHandler();
        }

        protected override void shutdownHandler()
        {
            base.shutdownHandler();
        }

        protected override void DisposeHook()
        {
            NanoleafPlugin.ControllerAdded -= NanoleafPlugin_ControllerAdded;
            base.DisposeHook();
        }

        private bool handleDimmerValue(HALHandleContext ctx)
        {
            if (!(ctx.Value is double)) return false;

            double d = ((double)ctx.Value).Limit(0, 100);
            this.DimmerValue = d / 100;
            ctx.AddHandleBag(new PropertyValueHandleBag(ctx.Property, d, this.HandlerName, this.getDependencies()));
            return true;
        }

        private bool handleStrobeValue(HALHandleContext ctx)
        {
            Strobe t = ctx.Value as Strobe;
            if (t == null) return false;

            this._lastStrobeContext = ctx;
            if (this._strobeEmulator == null)
            {
                this._strobeEmulator = new StrobeEmulator("Dimmer-Strobe");
                this._strobeEmulator.StrobeOff += handleEmulatorStrobeOff;
                this._strobeEmulator.StrobeOn += handleEmulatorStrobeOn;
            }

            this.StrobeValue = t.Speed;

            ctx.AddHandleBag(new PropertyValueHandleBag(ctx.Property, (Strobe)this.StrobeValue, this.HandlerName, this.getDependencies()));
            return true;
        }
        private void handleEmulatorStrobeOff(object sender, EventArgs args)
        {
            this._strobeOff = true;
            this.sendValueToController();
        }

        private void handleEmulatorStrobeOn(object sender, EventArgs args)
        {
            this._strobeOff = false;
            this.sendValueToController();
        }
        protected override object getPropBlackoutValue(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return 1.0;

            return base.getPropBlackoutValue(prop);
        }

        protected override object getPropHighlightValue(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return 100.0;

            return base.getPropHighlightValue(prop);
        }

        protected override IEnumerable<string> getDependencies()
        {
            yield return "";
        }
    }
}
