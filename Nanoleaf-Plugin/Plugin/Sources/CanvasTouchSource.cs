﻿using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Input.v2;
using System;
using System.Linq;

namespace Nanoleaf_Plugin
{
    public class CanvasTouchSource : AbstractInputSource
    {
        public string SerialNumber { get; private set; }
        public int PanelID { get; private set; }
        public ETouch TouchType { get; private set; }
        private long beatValue = 0;
        private CanvasTouchSource(string serialNumber, int panelID, ETouch touchType) :
            base(getID(serialNumber, panelID, touchType), getDisplayName(touchType), getCategory(serialNumber, panelID), default)
        {
            NanoleafMainSwitch.getInstance().EnabledChanged += CanvasTouchSource_EnabledChanged;
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
            Communication.StaticOnTouchEvent += ExternalControlEndpoint_StaticOnTouchEvent;
            SerialNumber = serialNumber;
            PanelID = panelID;
            TouchType = touchType;
        }

        private void CanvasTouchSource_EnabledChanged(object sender, System.EventArgs e)
        {
            AutofireChangedEvent = NanoleafMainSwitch.getInstance().Enabled;
        }

        public static CanvasTouchSource CreateHover(string serialNumber, int panelID)
        {
            return new CanvasTouchSource(serialNumber, panelID, ETouch.Hover);
        }
        public static CanvasTouchSource CreateDown(string serialNumber, int panelID)
        {
            return new CanvasTouchSource(serialNumber, panelID, ETouch.Down);
        }
        public static CanvasTouchSource CreateHold(string serialNumber, int panelID)
        {
            return new CanvasTouchSource(serialNumber, panelID, ETouch.Hold);
        }
        public static CanvasTouchSource CreateUp(string serialNumber, int panelID)
        {
            return new CanvasTouchSource(serialNumber, panelID, ETouch.Up);
        }
        public static CanvasTouchSource CreateSwipe(string serialNumber, int panelID)
        {
            return new CanvasTouchSource(serialNumber, panelID, ETouch.Swipe);
        }

        private void ExternalControlEndpoint_StaticOnTouchEvent(object sender, TouchEventArgs e)
        {
            try
            {
                if (!e.IP.Equals(NanoleafPlugin.getClient(this.SerialNumber)?.IP))
                    return;

                if (!e.TouchEvent.TouchPanelEvents.Any(ev => ev.PanelId.Equals(PanelID)))
                    return;

                var touch = e.TouchEvent.TouchPanelEvents.First(ev => ev.PanelId.Equals(PanelID));

                switch (TouchType)
                {
                    case ETouch.Hold:
                    case ETouch.Hover:
                        if (touch.Type == TouchType)
                            CurrentValue = true;
                        else
                            CurrentValue = false;
                        break;
                    default:
                        if (touch.Type == TouchType)
                        {
                            beatValue++;
                            CurrentValue = beatValue;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                NanoleafPlugin.Log.ErrorOrDebug(ex);
            }
        }

        private static string getID(string serialNumber, int panelID, ETouch part)
        {
            return $"Nanoleaf-{serialNumber}-Canvas:{panelID}-Touch:{part}";
        }
        private static string getDisplayName(ETouch part)
        {
            return part.ToString();
        }
        private static ParameterCategory getCategory(string serialNumber, int panelID)
        {
            return ParameterCategoryTools.FromNames("Nanoleaf", serialNumber, $"Canvas {panelID}");
        }
        public override EWellKnownInputType AutoGraphIOType
        {
            get
            {
                switch (TouchType)
                {
                    case ETouch.Hold:
                    case ETouch.Hover:
                        return EWellKnownInputType.Bool;
                    default:
                        return EWellKnownInputType.Beat;
                }
            }
        }

        public override object Min
        {
            get
            {
                switch (TouchType)
                {
                    case ETouch.Hold:
                    case ETouch.Hover:
                        return false;
                    default:
                        return 0;
                }
            }
        }

        public override object Max
        {
            get
            {
                switch (TouchType)
                {
                    case ETouch.Hold:
                    case ETouch.Hover:
                        return true;
                    default:
                        return long.MaxValue;
                }
            }
        }
    }
}
