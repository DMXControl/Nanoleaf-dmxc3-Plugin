using LumosLIB.Kernel.Log;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.Plugin;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = LumosLIB.Tools.I18n.DummyT;

namespace Nanoleaf_Plugin
{
    public class NanoleafPlugin : KernelPluginBase
    {
        internal static readonly ILumosLog Log = LumosLogger.getInstance(typeof(NanoleafPlugin));
        private static List<Controller> clients = new List<Controller>();
        private static List<Controller> clientsBindedToInputAssignment = new List<Controller>();
        private const string SETTINGS_CATEGORY_ID = "Settings:Nanoleaf";
        private const string NANOLEAF_AUTHTOKEN = "NANOLEAF.AUTHTOKEN.{0}";
        private bool isDisposed = false;
        private bool isStarted = false;
        internal static Controller getClient(string serialNumber)
        {
            if (serialNumber == null)
                return null;
            return clients?.FirstOrDefault(c => serialNumber.Equals(c.SerialNumber));
        }
        public NanoleafPlugin() : base("{25a96576-fda7-4297-bc59-6c4f2256ab6e}", "Nanoleaf-Plugin")
        {
#if DEBUG
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            Log.Info("Debugger attaced!");
#endif
            Communication.DeviceDiscovered += Communication_DeviceDiscovered;
        }

        private void Communication_DeviceDiscovered(object sender, DiscoveredEventArgs e)
        {
            Log.Info($"Device Discovered: {e.DiscoveredDevice.ToString()}");
            addControllerAsync(e.DiscoveredDevice);
        }
        private async Task addControllerAsync(DiscoveredDevice device, string authToken = null)
        {
            Controller controller = new Controller(device.IP, authToken);
            clients.Add(controller);
            await Task.Delay(100);
            Log.Info($"Controller Added: {controller.ToString()}");

            if (isStarted)
                bindInputAssignment();
        }
        private async Task bindInputAssignment()
        {
            var list = clients.Except(clientsBindedToInputAssignment).ToArray();
            foreach (var controller in list)
                try
                {
                    while (controller.SerialNumber == null)
                        await Task.Delay(1000);


                    var im = InputManager.getInstance();
                    im.RegisterSource(new CurrentPowerstateSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentBrightnessSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentCTSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentHueSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentSaturationSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentEffectSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentOrientationSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentNumberOfPanelsSource(controller.SerialNumber));
                    im.RegisterSource(new CurrentTouchedPanelsSource(controller.SerialNumber));

                    im.RegisterSource(CanvasGestureSource.CreateSingleTap(controller.SerialNumber));
                    im.RegisterSource(CanvasGestureSource.CreateDoubleTap(controller.SerialNumber));
                    im.RegisterSource(CanvasGestureSource.CreateSwipeDown(controller.SerialNumber));
                    im.RegisterSource(CanvasGestureSource.CreateSwipeUp(controller.SerialNumber));
                    im.RegisterSource(CanvasGestureSource.CreateSwipeRight(controller.SerialNumber));
                    im.RegisterSource(CanvasGestureSource.CreateSwipeLeft(controller.SerialNumber));

                    foreach (Panel panel in controller.Panels)
                    {
                        im.RegisterSource(CanvasPositionSource.CreateX(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasPositionSource.CreateY(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasPositionSource.CreateOrientation(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasTouchSource.CreateHover(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasTouchSource.CreateDown(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasTouchSource.CreateHold(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasTouchSource.CreateUp(controller.SerialNumber, panel.ID));
                        im.RegisterSource(CanvasTouchSource.CreateSwipe(controller.SerialNumber, panel.ID));
                        im.RegisterSink(new CanvasSink(controller.SerialNumber, panel));
                        im.RegisterSink(new CanvasSink(controller.SerialNumber, panel, true));
                    }
                    clientsBindedToInputAssignment.Add(controller);
                }
                catch (Exception e)
                {
                    Log.Error(string.Empty, e);
                }
        }
        protected override void initializePlugin()
        {
            SettingsManager s = SettingsManager.getInstance();
            s.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Show in InputAssignment"), string.Format(NANOLEAF_AUTHTOKEN, 0), String.Empty), string.Empty);
        }

        protected override void shutdownPlugin()
        {
            Log.Info("Shutdown");
            var im = InputManager.getInstance();
            im.UnregisterSinks(im.RegisteredSinks.Where(s => s.ID.Contains("Nanoleaf-")));
            im.UnregisterSources(im.RegisteredSources.Where(s => s.ID.Contains("Nanoleaf-")));
            isStarted = false;
        }

        protected override void startupPlugin()
        {
            Log.Info("Start");
            try
            {
                bindInputAssignment().GetAwaiter();
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
            }
            isStarted = true;
        }
        protected override void DisposePlugin(bool disposing)
        {
            base.DisposePlugin(disposing);
            isDisposed = true;
        }
    }
}
