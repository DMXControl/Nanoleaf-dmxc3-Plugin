using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CanvasPositionSource : AbstractInputSource
    {
        public enum EPositionPart
        {
            X, Y, Orientation
        }
        public string SerialNumber { get; private set; }
        public int PanelID { get; private set; }
        public EPositionPart Part { get; private set; }
        private CanvasPositionSource(string serialNumber, int panelID, EPositionPart part) :
            base(getID(serialNumber, panelID, part), getDisplayName(part), getCategory(serialNumber, panelID), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CanvasPositionSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnLayoutEvent += ExternalControlEndpoint_StaticOnLayoutEvent;
            SerialNumber = serialNumber;
            PanelID = panelID;
            Part = part;
            var position = NanoleafPlugin.getClient(SerialNumber).Panels.First(p => p.ID.Equals(panelID));
            switch (part)
            {
                case EPositionPart.X:
                    CurrentValue = position.X;
                    break;
                case EPositionPart.Y:
                    CurrentValue = position.Y;
                    break;
                case EPositionPart.Orientation:
                    CurrentValue = position.Orientation;
                    break;
            }
        }

        private void CanvasPositionSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        public static CanvasPositionSource CreateX(string serialNumber, int panelID)
        {
            return new CanvasPositionSource(serialNumber, panelID, EPositionPart.X);
        }
        public static CanvasPositionSource CreateY(string serialNumber, int panelID)
        {
            return new CanvasPositionSource(serialNumber, panelID, EPositionPart.Y);
        }
        public static CanvasPositionSource CreateOrientation(string serialNumber, int panelID)
        {
            return new CanvasPositionSource(serialNumber, panelID, EPositionPart.Orientation);
        }

        private void ExternalControlEndpoint_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            try
            {
                if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                    return;

                foreach (LayoutEvent _event in e.LayoutEvents.Events)
                {
                    if (_event.Layout == null)
                        continue;
                    Layout layout = _event.Layout.Value;
                    if (!layout.PanelPositions.Any(p => p.PanelId.Equals(PanelID)))
                        continue;

                    var position = layout.PanelPositions.First(p => p.PanelId.Equals(PanelID));

                    switch (Part)
                    {
                        case EPositionPart.X:
                            CurrentValue = position.X;
                            break;
                        case EPositionPart.Y:
                            CurrentValue = position.Y;
                            break;
                        case EPositionPart.Orientation:
                            CurrentValue = position.Orientation;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                NanoleafPlugin.Log.ErrorOrDebug(ex);
            }
        }

        private static string getID(string serialNumber, int panelID, EPositionPart part)
        {
            return $"Nanoleaf-{serialNumber}-Canvas:{panelID}-Position:{part}";
        }
        private static string getDisplayName(EPositionPart part)
        {
            return part.ToString();
        }
        private static ParameterCategory getCategory(string serialNumber, int panelID)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber, $"Canvas {panelID}");
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get { return EWellKnownInputType.Numeric; }
        }

        public override object Min
        {
            get { return 0; }
        }

        public override object Max
        {
            get
            {
                switch (Part)
                {
                    case EPositionPart.X:
                    case EPositionPart.Y:
                        return 50000;
                    case EPositionPart.Orientation:
                        return 360;
                }
                return 0;
            }
        }
    }
}
