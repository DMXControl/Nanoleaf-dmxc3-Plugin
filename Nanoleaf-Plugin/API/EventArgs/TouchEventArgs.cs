using System;

namespace Nanoleaf_Plugin.API
{
    public class TouchEventArgs : EventArgs
    {
        public readonly string IP;
        public readonly TouchEvent TouchEvent;
        public TouchEventArgs(string ip, TouchEvent touchEvent)
        {
            IP = ip;
            TouchEvent = touchEvent;
        }
    }
}
