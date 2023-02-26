using LumosLIB.Tools;
using Nanoleaf_Plugin.Plugin.Device;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.DeviceProperties;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.HAL.Handler;
using org.dmxc.lumos.Kernel.HAL.Handler.Helper;
using org.dmxc.lumos.Kernel.PropertyType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoleaf_Plugin
{
    sealed class NanoleafControllerHandlerNode : AbstractHandlerNode
    {
        public static readonly string DMXC_CONTROLLED_NAME = "DMXC3 controlled";
        private string serialNumber;
        private EDeviceType deviceType = EDeviceType.UNKNOWN;
        private Controller _instance = null;
        private double dimmerValue;
        private string effectValue;
        public double DimmerValue
        {
            get { return this.dimmerValue; }
            set
            {
                this.dimmerValue = value;
                sendDimmerValueToController();
                    
            }
        }

        public string EffectValue
        {
            get { return this.effectValue; }
            private set
            {
                this.effectValue = value;
                sendEffectValueToController();
            }
        }

        public bool DMXCControlled => EffectValue.Contains(DMXC_CONTROLLED_NAME, StringComparison.OrdinalIgnoreCase);

        private readonly List<string> _effectList = new List<string>();

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
        private RawStepProperty EffectProperty
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
                    serialNumber = ((NanoleafControllerDevice)value.ParentDevice).SerialNumber;
                    deviceType = ((NanoleafControllerDevice)value.ParentDevice).DeviceType;
                    this._instance = NanoleafPlugin.getClient(serialNumber);
                    updateEffectList();
                }
                else
                    throw new ArgumentException("This Type of Handler needs to be assigned to a " + NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME);
            }
        }

        private void updateEffectList()
        {
            _effectList.Clear();
            _effectList.Add(DMXC_CONTROLLED_NAME);

            if (_instance == null)
                return;

            _effectList.AddRange(_instance.EffectList.Where(e => !e.Contains("*ExtControl*", StringComparison.OrdinalIgnoreCase)));
        }

        private StrobeEmulator _strobeEmulator;
        private HALHandleContext _lastStrobeContext;
        private NanoleafControllerHandlerNode()
            : base()
        {
            NanoleafPlugin.ControllerAdded += NanoleafPlugin_ControllerAdded;
        }

        private void NanoleafPlugin_ControllerAdded(object sender, EventArgs e)
        {
            if (this._instance == null && !string.IsNullOrWhiteSpace(serialNumber))
            {
                this.setInstance();
            }
        }

        protected override PropertyHandlerWorker getPropertyWorker(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return new PropertyHandlerWorker(handleDimmerValue);
            if (prop is RawStepProperty)
                return new PropertyHandlerWorker(handleRawValue);

            return null;
        }

        protected override List<PropertyDependencyBag> PropHandleProperties
        {
            get
            {
                if (this.DimmerProperty == null)
                {
                    this.DimmerProperty = new DimmerProperty(this.ParentBeam);
                }
                if (this.EffectProperty == null)
                {
                    this.EffectProperty = new RawStepProperty(this.ParentBeam, "Scene");
                }

                List<PropertyDependencyBag> props = new List<PropertyDependencyBag>();
                props.Add(new PropertyDependencyBag(this.DimmerProperty, getDependencies(),
                    this.ParentHandler is AbstractDMXHandlerNode));

                props.Add(new PropertyDependencyBag(this.EffectProperty, getDependencies(),
                    this.ParentHandler is AbstractDMXHandlerNode));

                return props;
            }
        }

        protected override List<PropertyDependencyBag> OptionalPropHandleProperties
        {
            get
            {
                if (this.StrobeProperty == null)
                {
                    this.StrobeProperty = new StrobeProperty(this.ParentBeam);
                }

                List<PropertyDependencyBag> optProps = new List<PropertyDependencyBag>();

                optProps.Add(new PropertyDependencyBag(this.StrobeProperty, getDependencies(),
                    this.ParentHandler is AbstractDMXHandlerNode));

                return optProps;
            }
        }

        private bool canSendValueToController() => this._instance != null && NanoleafMainSwitch.getInstance().Enabled;

        private void sendDimmerValueToController()
        {
            if(canSendValueToController())
                _instance.Brightness = (ushort)(dimmerValue * 100);
        }

        private void sendEffectValueToController()
        {
            if (canSendValueToController())
                _instance.SetEffect(DMXCControlled, EffectValue);
        }

        protected override IPropertyType getPropTypeInstance(IDeviceProperty prop)
        {
            if (prop is DimmerProperty)
                return new IntensityType<double>(0.0, 100.0);
            if (prop is StrobeProperty)
                return new StrobeType(StrobeEmulator.EMULATOR_MIN, StrobeEmulator.EMULATOR_MAX);
            if (prop is RawStepProperty)
            {
                var c = new StringEnumType(this._effectList);
                if (this.DefaultVal != null)
                {
                    var n = c.EnumValues.ElementAtOrDefault((int)this.DefaultVal.Value);
                    if (n != null)
                        c.Value = n;
                }
                else if (this.DefaultValString != null)
                {
                    var n = c.EnumValues.FirstOrDefault(e => Equals(e.ID, this.DefaultValString));
                    if (n != null)
                        c.Value = n;
                }
                return c;
            }
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
                this.serialNumber = d.SerialNumber;
                this.setInstance();
                updateEffectList();
            }
        }
        private void setInstance()
        {
            this._instance = NanoleafPlugin.getClient(serialNumber);
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

        private bool handleRawValue(HALHandleContext ctx)
        {
            if (ctx.Value is EnumString st)
            {
                this.EffectValue = st.ToString();
                ctx.AddHandleBag(new PropertyValueHandleBag(ctx.Property, st, this.HandlerName, this.getDependencies()));
                return true;
            }
            return false;
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
        private IEnumerable<DMXRangeWithDiscreteValue> getPossibleValues(EHandlerValueType handlerType)
        {
            return this.getNestedHandlerValues(this, handlerType, typeof(DMXRangeWithDiscreteValue))
                .Cast<DMXRangeWithDiscreteValue>();
        }
    }
}
