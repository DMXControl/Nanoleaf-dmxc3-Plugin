using System;

namespace Nanoleaf_Plugin.API
{
    public class DiscoveredEventArgs : EventArgs
    {
        public readonly DiscoveredDevice DiscoveredDevice;
        public DiscoveredEventArgs(DiscoveredDevice discoveredDevice)
        {
            DiscoveredDevice = discoveredDevice;
        }
    }
}
