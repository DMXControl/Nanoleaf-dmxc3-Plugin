using LumosLIB.Kernel;
using LumosLIB.Kernel.Log;
using LumosLIB.Tools;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Nanoleaf_Plugin.Plugin.Device
{
    public class NanoleafControllerDevice : AbstractDevice
    {
        public static readonly string NANOLEAF_CONTROLLER_TYPE_NAME = "NanoleafController";
        public static readonly string CONTROLLER_ID_PARAMETER = "Controller ID";
        public static readonly string CONTROLLER_TYPE_PARAMETER = "ControllerType";
        private static readonly ILumosLog log = LumosLogger.getInstance(nameof(NanoleafDevice));

        private string _serialNumber;
        public event EventHandler ControllerIDChanged;

        public NanoleafControllerDevice(string id) : base(id, null, false, NANOLEAF_CONTROLLER_TYPE_NAME)
        {
            XmlDocument d = new XmlDocument();
            XmlNode node = d.CreateElement("root");
            node.AppendChild(d.CreateElement("nanoleaf-controller"));

            DDFParseContext ctx = new DDFParseContext(node)
            {
                IgnoreMissingHALNodes = false,
                NotPropagateXMLParseException = true
            };

            DeviceBeam b = new DeviceBeam(this, 1, ctx);
            this.addBeam(b);
        }

        public string SerialNumber
        {
            get { return this._serialNumber; }
            internal set
            {
                if (value != this._serialNumber)
                {
                    this._serialNumber = value;
                    if (ControllerIDChanged != null)
                        ControllerIDChanged(this, EventArgs.Empty);
                }
            }
        }

        private EDeviceType deviceType = EDeviceType.UNKNOWN;
        public EDeviceType DeviceType
        {
            get { return this.deviceType; }
            internal set
            {
                this.deviceType = value;
                if (value != EDeviceType.UNKNOWN)
                    this.Image = value.ToString();
                else this.Image = null;
            }
        }

        protected override IEnumerable<GenericParameter> ParametersInternal
        {
            get
            {
                List<GenericParameter> ret = new List<GenericParameter>();
                try
                {
                    var controllers = NanoleafPlugin.getControllers().Select(p => p.SerialNumber.ToString()).ToArray();
                    ret.Add(new GenericParameter(CONTROLLER_TYPE_PARAMETER, DeviceParameters.DeviceParameterType, typeof(string), EGenericParameterOptions.HIDDEN));
                    ret.Add(new GenericParameter(CONTROLLER_ID_PARAMETER, DeviceParameters.DeviceParameterType, typeof(string), EGenericParameterOptions.PERSISTANT, controllers));
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                return ret.AsEnumerable();
            }
        }

        protected override object getParameterInternal(GenericParameter parameter)
        {
            try
            {
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_TYPE_PARAMETER))
                    return this.DeviceType.ToString();
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_ID_PARAMETER))
                    return this.SerialNumber;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return base.getParameterInternal(parameter);
        }

        protected override bool setParameterInternal(GenericParameter parameter, object value, out object valueToSend)
        {
            try
            {
                valueToSend = null;
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_TYPE_PARAMETER))
                {
                    if (value is EDeviceType dt)
                        this.DeviceType = dt;
                    else if (value is string str)
                        this.DeviceType = (EDeviceType)Enum.Parse(typeof(EDeviceType), str);
                    else if (value is int i)
                        this.DeviceType = (EDeviceType)i;
                }
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_ID_PARAMETER))
                {
                    if (value is string id)
                    {
                        valueToSend = this.SerialNumber = id;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return base.setParameterInternal(parameter, value, out valueToSend);
        }

        protected override bool testParameterInternal(GenericParameter parameter, object value)
        {
            try
            {
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_TYPE_PARAMETER))
                {
                    if (value is EDeviceType dt) return true;
                    else if (value is string str) return true;
                    else if (value is int i) return true;
                }
                if (parameter.Name.EqualsIgnoreCase(CONTROLLER_ID_PARAMETER))
                {
                    try
                    {
                        var val = LumosTools.TryConvertToInt32(value);
                        return val.HasValue;
                    }
                    catch { return false; }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return base.testParameterInternal(parameter, value);
        }
    }
}
