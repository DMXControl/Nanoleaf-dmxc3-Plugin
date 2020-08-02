using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Nanoleaf_Plugin.API
{
    public enum ETouch
    {
        Hover,
        Down,
        Hold,
        Up,
        Swipe,
        UNKNOWN,
    }
    public class TouchEvent
    {
        public int TouchedPanelsNumber { get; private set; }
        private List<TouchPanelEvent> touchPanelEvents = new List<TouchPanelEvent>();
        public ReadOnlyCollection<TouchPanelEvent> TouchPanelEvents
        {
            get
            {
                return touchPanelEvents.AsReadOnly();
            }
        }

        private TouchEvent(byte[] array)
        {
            this.TouchedPanelsNumber = System.BitConverter.ToInt16(new[] { array[1], array[0] }, 0);
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(array, 2, array.Length-2);
                ms.Position = 0;
                for (int i = 0; i < TouchedPanelsNumber; i++)
                {
                    buffer = new byte[5];
                    ms.Read(buffer, 0, buffer.Length);
                    this.touchPanelEvents.Add(TouchPanelEvent.FromArray(buffer));
                }
            }
        }
        public static TouchEvent FromArray(byte[] array)
        {
            return new TouchEvent(array);
        }
        public class TouchPanelEvent
        {
            public int PanelId { get; private set; }
            public int? PanelIdSwipedFrom { get; private set; } = null;
            public ETouch Type { get; private set; }
            public double Strength { get; private set; }
            private TouchPanelEvent(byte[] array)
            {
                this.PanelId = System.BitConverter.ToUInt16(new[] { array[1], array[0] }, 0);
                if (array[4] != byte.MaxValue || array[3] != byte.MaxValue)
                    this.PanelIdSwipedFrom = System.BitConverter.ToUInt16(new[] { array[4], array[3] }, 0);

                var bit0 = getBit(array[2], 1);
                var bit1 = getBit(array[2], 2);
                var bit2 = getBit(array[2], 3);
                var bit3 = getBit(array[2], 4);
                var bit4 = getBit(array[2], 5);
                var bit5 = getBit(array[2], 6);
                var bit6 = getBit(array[2], 7);
                var bit7 = getBit(array[2], 8);


                Strength = ((bit0 ? 1 : 0) + (bit1 ? 2 : 0) + (bit2 ? 4 : 0) + (bit3 ? 8 : 0)) / 15.0;
                int typ = (bit4 ? 1 : 0) + (bit5 ? 2 : 0) + (bit6 ? 4 : 0);
                Type = (ETouch)typ;
            }
            public static TouchPanelEvent FromArray(byte[] array)
            {
                return new TouchPanelEvent(array);
            }
            private bool getBit(byte b, byte bitNumber)
            {
                return (b & (1 << bitNumber - 1)) != 0;
            }
        }
    }
}
