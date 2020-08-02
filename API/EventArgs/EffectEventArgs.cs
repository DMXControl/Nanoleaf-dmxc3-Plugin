using System;

namespace Nanoleaf_Plugin.API
{
    public class EffectEventArgs : EventArgs
    {
        public readonly string IP;
        public readonly EffectEvents EffectEvents;
        public EffectEventArgs(string ip, EffectEvents effectEvents)
        {
            IP = ip;
            EffectEvents = effectEvents;
        }
    }
}
