using System;

namespace Nanoleaf_Plugin.API
{
    public readonly struct DiscoveredDevice
    {
        public readonly string IP;
        public readonly string Name;
        public readonly EDeviceType DeviceTyp;
        public DiscoveredDevice(string ip, string name, EDeviceType deviceType) :this()
        {
            IP = ip;
            Name = name;
            DeviceTyp = deviceType;
        }
        public override string ToString()
        {
            return $"{Name} {IP} {DeviceTyp}";
        }
    }
}
