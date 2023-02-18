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

namespace Nanoleaf_Plugin
{
    sealed class NanoleafHandlerNode : AbstractHandlerNode
    {
        private int panelId = 0;
        private EDeviceType deviceType = EDeviceType.UNKNOWN;
        private Panel _instance = null;
        private System.Drawing.Color colorValue;
        public System.Drawing.Color ColorValue
        {
            get { return this.colorValue; }
            set
            {
                this.colorValue = value;
                this.sendValueToPanel();
            }
        }
        private double dimmerValue;
        public double DimmerValue
        {
            get { if (!this._strobeOff || this.StrobeValue == 0) return this.dimmerValue; else return 0; }
            set
            {
                this.dimmerValue = value;
                this.sendValueToPanel();
            }
        }
        public double StrobeValue
        {
            get { return this._strobeEmulator == null ? 0.0 : this._strobeEmulator.Frequency; }
            private set
            {
                if (this._strobeEmulator != null)
                    this._strobeEmulator.Frequency = value;

                this.sendValueToPanel();
            }
        }


        private ColorProperty ColorProperty
        {
            get;
            set;
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
                if (value != null && value.ParentDevice.Type.Equals(NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME))
                {
                    //Setting base.ParentBeam throws an Exception when Beam has already been set.
                    base.ParentBeam = value;
                    ((NanoleafDevice)value.ParentDevice).PanelIDChanged += new EventHandler(NanoleafHandlerNode_PanelIDChanged);
                    panelId = ((NanoleafDevice)value.ParentDevice).PanelID;
                    deviceType = ((NanoleafDevice)value.ParentDevice).DeviceType;
                    this._instance = NanoleafPlugin.getAllPanels(deviceType).FirstOrDefault(p=>p.ID.Equals(panelId));
                }
                else
                    throw new ArgumentException("This Type of Handler needs to be assigned to a " + NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME);
            }
        }

        private bool _strobeOff;
        private StrobeEmulator _strobeEmulator;
        private HALHandleContext _lastStrobeContext;
        private NanoleafHandlerNode()
            : base()
        {
            NanoleafPlugin.ControllerAdded += NanoleafPlugin_ControllerAdded;
        }

        private void NanoleafPlugin_ControllerAdded(object sender, EventArgs e)
        {
            if (this._instance == null && panelId != 0)
            {
                this.setInstance();
            }
        }

        protected override PropertyHandlerWorker getPropertyWorker(IDeviceProperty prop)
        {
            if (prop is ColorProperty)
                return new PropertyHandlerWorker(handleColorValue);
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
                if (this.ColorProperty == null)
                {
                    this.ColorProperty = new ColorProperty(this.ParentBeam, hasRgb: true);
                }
                props.Add(new PropertyDependencyBag(this.ColorProperty, getDependencies()));
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
        private void sendValueToPanel()
        {
            if (this._instance != null)
                NanoleafPlugin.getControllerFromPanel(this._instance.ID)?.SetPanelColor(this._instance.ID, new Panel.RGBW((byte)(this.colorValue.R * this.DimmerValue), (byte)(this.colorValue.G * this.DimmerValue), (byte)(this.colorValue.B * this.DimmerValue)));
        }
        protected override IPropertyType getPropTypeInstance(IDeviceProperty prop)
        {
            if (prop is ColorProperty)
                return new ColorType()
                {
                    ColorValue = LumosColor.White
                };
            if (prop is DimmerProperty)
                return new IntensityType<double>(0.0, 100.0);
            if (prop is StrobeProperty)
                return new StrobeType(StrobeEmulator.EMULATOR_MIN, StrobeEmulator.EMULATOR_MAX);

            return null;
        }

        protected override IDeviceProperty SingleHandleProperty
        {
            get { return this.ColorProperty; }
        }

        private void NanoleafHandlerNode_PanelIDChanged(object sender, EventArgs e)
        {
            var d = sender as NanoleafDevice;
            if (d != null)
            {
                this.deviceType = d.DeviceType;
                this.panelId = d.PanelID;
                this.setInstance();
            }
        }
        private void setInstance()
        {
            this._instance = NanoleafPlugin.getAllPanels(this.deviceType).FirstOrDefault(p => p.ID.Equals(this.panelId));

            NanoleafPlugin.getControllers().ForEach(c => c.PanelAdded -= PanelAdded);
            NanoleafPlugin.getControllers().ForEach(c => c.PanelRemoved -= PanelRemoved);

            if (this._instance == null)
                NanoleafPlugin.getControllers().ForEach(c => c.PanelAdded += PanelAdded);
            else
                NanoleafPlugin.getControllers().ForEach(c => c.PanelRemoved += PanelRemoved);

        }

        private void PanelAdded(object sender, EventArgs e)
        {
            if (this._instance == null)
                setInstance();
        }
        private void PanelRemoved(object sender, EventArgs e)
        {
            if (this._instance != null)
                setInstance();
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
            NanoleafPlugin.getControllers().ForEach(c => c.PanelAdded -= PanelAdded);
            NanoleafPlugin.getControllers().ForEach(c => c.PanelRemoved -= PanelRemoved);
            NanoleafPlugin.ControllerAdded -= NanoleafPlugin_ControllerAdded;
            base.DisposeHook();
        }

        private bool handleColorValue(HALHandleContext ctx)
        {
            if (this._instance == null)
                return false;

            System.Drawing.Color? c = null;

            if (ctx.Value is LumosColor lc)
                c = lc.ColorValue;
            else if (ctx.Value is System.Drawing.Color _c)
                c = _c;

            if (c.HasValue)
            {
                this.ColorValue = c.Value;
                return true;
            }
            return false;
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
                this._strobeEmulator = new StrobeEmulator("RGB-Color-Strobe");
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
            this.sendValueToPanel();
        }

        private void handleEmulatorStrobeOn(object sender, EventArgs args)
        {
            this._strobeOff = false;
            this.sendValueToPanel();
        }
        protected override object getPropBlackoutValue(IDeviceProperty prop)
        {
            if (prop is ColorProperty)
                return LumosColor.BLACKOUT_VALUE;

            if (prop is DimmerProperty)
                return 0.0;

            return base.getPropBlackoutValue(prop);
        }

        protected override object getPropHighlightValue(IDeviceProperty prop)
        {
            if (prop is ColorProperty)// && this.ColorValue.ColorsEqual(_Color.Black))
                return LumosColor.HIGHLIGHT_VALUE;

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
