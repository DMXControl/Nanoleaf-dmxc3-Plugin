using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;
using static Nanoleaf_Plugin.API.GestureEvent;

namespace Nanoleaf_Plugin
{
    public class CanvasGestureSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public EGesture GestureType { get; private set; }
        private long beatValue = 0;
        private CanvasGestureSource(string serialNumber, EGesture gestureType) :
            base(getID(serialNumber, gestureType), getDisplayName(gestureType), getCategory(serialNumber))
        {
            Communication.StaticOnGestureEvent += ExternalControlEndpoint_StaticOnGestureEvent;
            SerialNumber = serialNumber;
            GestureType = gestureType;
            CurrentValue = beatValue;
        }

        public static CanvasGestureSource CreateSingleTap(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.SingleTap);
        }
        public static CanvasGestureSource CreateDoubleTap(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.DoubleTap);
        }
        public static CanvasGestureSource CreateSwipeDown(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.SwipeDown);
        }
        public static CanvasGestureSource CreateSwipeUp(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.SwipeUp);
        }
        public static CanvasGestureSource CreateSwipeLeft(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.SwipeLeft);
        }
        public static CanvasGestureSource CreateSwipeRight(string serialNumber)
        {
            return new CanvasGestureSource(serialNumber, EGesture.SwipeRight);
        }

        private void ExternalControlEndpoint_StaticOnGestureEvent(object sender, GestureEventArgs e)
        {
            if (!NanoleafPlugin.getClient(this.SerialNumber).IP.Equals(e.IP))
                return;

            GestureEvents events = e.GestureEvents;
            if (events == null)
                return;
            var val = events.Events.FirstOrDefault(g => g.Gesture == GestureType);
            if (val!=null)
            {
                beatValue++;
                CurrentValue = beatValue;
            }
        }

        private static string getID(string serialNumber, EGesture part)
        {
            return $"Nanoleaf-{serialNumber}-Gesture:{part}";
        }
        private static string getDisplayName(EGesture part)
        {
            return part.ToString();
        }
        private static ParameterCategory getCategory(string serialNumber)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber);
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get
            {
                return EWellKnownInputType.Beat;
            }
        }

        public override object Min
        {
            get
            {
                return 0;
            }
        }

        public override object Max
        {
            get
            {
                return long.MaxValue;
            }
        }
    }
}
