using LumosToolsLIB.Tools;
using org.dmxc.lumos.Kernel.MainSwitch;
using System;

namespace Nanoleaf_Plugin.Plugin.MainSwitch
{
    public class NanoleafMainSwitch : IMainSwitch
    {
        private static readonly NanoleafMainSwitch instance = new();
        public static NanoleafMainSwitch getInstance() => instance;
        private bool _enabled;
        public event EventHandler<EventArgs> EnabledChanged;

        public string ID => "NanoleafMainSwitch";

        public string Name => "Nanoleafs";

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    this.EnabledChanged?.InvokeFailSafe(this, EventArgs.Empty);
                }
            }
        }
    }
}
