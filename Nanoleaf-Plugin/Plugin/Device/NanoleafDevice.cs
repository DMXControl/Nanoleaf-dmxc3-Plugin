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

namespace Nanoleaf_Plugin
{
    sealed class NanoleafDevice : AbstractDevice
    {
        public static readonly string NANOLEAF_DEVICE_TYPE_NAME = "NanoleafDevice";
        public static readonly string PANEL_ID_PARAMETER = "Panel ID";
        public static readonly string DEVICE_TYPE_PARAMETER = "DeviceType";
        private static readonly ILumosLog log = LumosLogger.getInstance(nameof(NanoleafDevice));

        private int _panleId;
        public event EventHandler PanelIDChanged;

        public NanoleafDevice(string id)
           : base(id, null, false, NANOLEAF_DEVICE_TYPE_NAME)
        {
            XmlDocument d = new XmlDocument();
            XmlNode node = d.CreateElement("root");
            node.AppendChild(d.CreateElement("nanoleaf"));

            DDFParseContext ctx = new DDFParseContext(node)
            {
                IgnoreMissingHALNodes = false,
                NotPropagateXMLParseException = true
            };

            DeviceBeam b = new DeviceBeam(this, 1, ctx);
            this.addBeam(b);
        }

        public int PanelID
        {
            get { return this._panleId; }
            internal set
            {
                if (value != this._panleId)
                {
                    this._panleId = value;
                    if (PanelIDChanged != null)
                        PanelIDChanged(this, EventArgs.Empty);
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
                    var panels = NanoleafPlugin.getAllPanels(this.DeviceType).Select(p => p.ID.ToString()).ToArray();
                    ret.Add(new GenericParameter(DEVICE_TYPE_PARAMETER, DeviceParameters.DeviceParameterType, typeof(string), EGenericParameterOptions.HIDDEN));
                    ret.Add(new GenericParameter(PANEL_ID_PARAMETER, DeviceParameters.DeviceParameterType, typeof(string), EGenericParameterOptions.PERSISTANT, panels));
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
                if (parameter.Name.EqualsIgnoreCase(DEVICE_TYPE_PARAMETER))
                    return this.DeviceType.ToString();
                if (parameter.Name.EqualsIgnoreCase(PANEL_ID_PARAMETER))
                    return this.PanelID;
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
                if (parameter.Name.EqualsIgnoreCase(DEVICE_TYPE_PARAMETER))
                {
                    if (value is EDeviceType dt)
                        this.DeviceType = dt;
                    else if (value is string str)
                        this.DeviceType = (EDeviceType)Enum.Parse(typeof(EDeviceType), str);
                    else if (value is int i)
                        this.DeviceType = (EDeviceType)i;
                }
                if (parameter.Name.EqualsIgnoreCase(PANEL_ID_PARAMETER))
                {
                    //int for backward compatibility as DesklampID previous was an Integer
                    if (value is int id)
                    {
                        valueToSend = this.PanelID = id;
                        return true;
                    }
                    else
                    {
                        var val = LumosTools.TryConvertToInt32(value);
                        if (val.HasValue)
                        {
                            this.PanelID = val.Value;
                            return true;
                        }
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
                if (parameter.Name.EqualsIgnoreCase(DEVICE_TYPE_PARAMETER))
                {
                    if (value is EDeviceType dt) return true;
                    else if (value is string str) return true;
                    else if (value is int i) return true;
                }
                if (parameter.Name.EqualsIgnoreCase(PANEL_ID_PARAMETER))
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