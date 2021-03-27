using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LumosLIB.Kernel;
using LumosLIB.Kernel.Log;
using LumosLIB.Tools;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.HAL;
using org.dmxc.lumos.Kernel.Project;

namespace Nanoleaf_Plugin
{
    sealed class NanoleafDevice : AbstractDevice
    {
        public static readonly string NANOLEAF_DEVICE_TYPE_NAME = "NanoleafDevice";
        public static readonly string PANEL_ID_PARAMETER = "Panel ID";
        private static readonly ILumosLog log = LumosLogger.getInstance(typeof(NanoleafDevice));

        private int _panleId;
        public event EventHandler PanelIDChanged;

        public NanoleafDevice(string id)
            : base(id, null)
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

        public override string Type => NANOLEAF_DEVICE_TYPE_NAME;

        public override bool IsWrappingDevice => true;

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

        public override int SaveLoadOrder
        {
            get
            {
                int _default = base.SaveLoadOrder + 1;
                var mp = this.Properties.FirstOrDefault(c => c.PropertyType == org.dmxc.lumos.Kernel.DeviceProperties.EPropertyType.Color);
                if (mp == null) return _default;

                var p = mp.getParameter("Patching") as DevicePatching;
                if (p == null) return _default;

                var devs = p.AllPatches
                    .Select(c => DeviceManager.getInstance().GetDeviceByID(c))
                    .NotNull()
                    .ToList();

                if (devs.Count == 0) return _default;

                var max = devs.Max(c => c.SaveLoadOrder) + 1;
                return max;
            }
        }


        protected override IEnumerable<DeviceParameter> ParametersInternal
        {
            get
            {
                yield return new DeviceParameter(PANEL_ID_PARAMETER, typeof(string),
                    EGenericParameterOptions.PERSISTANT, NanoleafPlugin.getAllPanels().Select(p => (object)p.ID.ToString()).ToArray());
            }
        }

        protected override object getParameterInternal(DeviceParameter parameter)
        {
            if (parameter.Name.EqualsIgnoreCase(PANEL_ID_PARAMETER))
                return this.PanelID;
            return base.getParameterInternal(parameter);
        }

        protected override bool setParameterInternal(DeviceParameter parameter, object value, out object valueToSend)
        {
            valueToSend = null;
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
                    var val=LumosTools.TryConvertToInt32(value);
                    if (val.HasValue)
                    {
                        this.PanelID = val.Value;
                        return true;
                    }
                }
            }

            return base.setParameterInternal(parameter, value, out valueToSend);
        }

        protected override bool testParameterInternal(DeviceParameter parameter, object value)
        {
            if (parameter.Name.EqualsIgnoreCase(PANEL_ID_PARAMETER))
            {
                try
                {
                    var val = LumosTools.TryConvertToInt32(value);
                    return val.HasValue;
                }
                catch { return false; }
            }
            return base.testParameterInternal(parameter, value);
        }
    }
}
