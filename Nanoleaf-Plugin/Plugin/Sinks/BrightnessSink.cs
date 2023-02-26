using LumosLIB.Kernel;
using LumosLIB.Tools;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.PropertyType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_Plugin.Plugin.Sinks
{
    public class BrightnessSink : AbstractInputSink
    {
        public readonly string SerialNumber;
        private object valueCache;

        public BrightnessSink(string serialNumber, bool usePanelID = false) :
            base(getID(serialNumber), getDisplayName(), getCategory(serialNumber))

        {
            NanoleafMainSwitch.getInstance().EnabledChanged += BrightnessSink_EnabledChanged; ;
            SerialNumber = serialNumber;
        }

        private void BrightnessSink_EnabledChanged(object sender, EventArgs e)
        {
            if (NanoleafMainSwitch.getInstance().Enabled)
                UpdateValue(valueCache);
        }

        private static string getID(string serialNumber)
        {
            return string.Format("Nanoleaf-{0}-SetBrightness", serialNumber);
        }

        private static string getDisplayName()
        {
            return "Brightness";
        }

        private static ParameterCategory getCategory(string serialNumber)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber);
        }

        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.Numeric; }
        }

        public override object Min => 0;

        public override object Max => 100;

        public override bool UpdateValue(object newValue)
        {
            valueCache = newValue;

            if (!NanoleafMainSwitch.getInstance().Enabled)
                return true;

            if (newValue is null)
                return false;

            uint? brightness = (ushort)LumosTools.TryConvertToDouble(newValue!);

            if (!brightness.HasValue)
                return false;

            try
            {
                var Controler = NanoleafPlugin.getClient(SerialNumber);
                Controler.Brightness = (ushort)brightness!;
                return true;
            }
            catch (Exception e)
            {
                NanoleafPlugin.Log.Error(string.Empty, e);
                return false;
            }
        }
    }
}
