using System;

namespace Nanoleaf_Plugin.API
{
    public class GestureEventArgs : EventArgs
    {
        public readonly string IP;
        public readonly GestureEvents GestureEvents;
        public GestureEventArgs(string ip, GestureEvents gestureEvents)
        {
            IP = ip;
            GestureEvents = gestureEvents;
        }
    }
}
