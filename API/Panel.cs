using System;
using System.Drawing;
using System.Linq;
using static Nanoleaf_Plugin.API.PanelPosition;

namespace Nanoleaf_Plugin.API
{
    public class Panel
    {
        public string IP { get; private set; }
        public int ID { get; private set; }
        private int x;
        public int X
        {
            get { return x; }
            private set
            {
                if (x == value)
                    return;

                x = value;
                XChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private int y;
        public int Y
        {
            get { return y; }
            private set
            {
                if (y == value)
                    return;

                y = value;
                YChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private int orientation;
        public int Orientation
        {
            get { return orientation; }
            private set
            {
                if (orientation == value)
                    return;

                orientation = value;
                OrientationChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public EShapeType Shape { get; private set; }

        private RGBW streamingColor;
        public RGBW StreamingColor
        {
            get { return streamingColor; }
            set
            {
                if (streamingColor == value)
                    return;

                streamingColor = value;
            }
        }

        public double SideLength { get; internal set; }

        public event EventHandler XChanged;
        public event EventHandler YChanged;
        public event EventHandler OrientationChanged;

        public Panel(string ip, PanelPosition pp,ushort sideLength)
        {
            IP = ip;
            ID = pp.PanelId;
            X = pp.X;
            Y = pp.Y;
            Orientation = pp.Orientation;
            Shape = pp.ShapeType;
            SideLength = sideLength;
            Communication.StaticOnLayoutEvent += Communication_StaticOnLayoutEvent;
        }

        private void Communication_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            var pp = e.LayoutEvent.Layout.PanelPositions.FirstOrDefault(p => p.PanelId.Equals(ID));
            if (pp != null)
            {
                X = pp.X;
                Y = pp.Y;
                Orientation = pp.Orientation;
            }
        }

        public override string ToString()
        {
            return $"ID: {ID} X: {X} Y: {y} Orientation: {Orientation}";
        }
        public readonly struct RGBW
        {
            public readonly byte R;
            public readonly byte G;
            public readonly byte B;
            public readonly byte W;

            public RGBW(byte r, byte g, byte b) : this()
            {
                R = r;
                G = g;
                B = b;
            }

            public RGBW(byte r, byte g, byte b, byte w) : this()
            {
                R = r;
                G = g;
                B = b;
                W = w;
            }
            public static bool operator ==(RGBW c1, RGBW c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(RGBW c1, RGBW c2)
            {
                return !c1.Equals(c2);
            }
            public override string ToString()
            {
                return $"{R}; {G}; {B}; {W}";
            }
        }
    }
}
