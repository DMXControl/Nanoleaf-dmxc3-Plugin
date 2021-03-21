using LumosLIB.Kernel.Log;
using Nanoleaf_Plugin.API;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.Net;
using org.dmxc.lumos.Kernel.Plugin;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        private const string NANOLEAF_SHOW_IN_INPUTASSIGNMENT = "NANOLEAF.SHOW_IN_INPUTASSIGNMENT";
        private const string NANOLEAF_KNOWN_CONTROLER_IPS = "NANOLEAF.KNOWN_CONTROLER_IPS";
        private const string NANOLEAF_AUTHTOKEN = "NANOLEAF.AUTHTOKEN:{0}";
        private bool isDisposed = false;
        private bool isStarted = false;
        private string ips = "";
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
            SettingsManager sm = SettingsManager.getInstance();
            string ip = e.DiscoveredDevice.IP;
            string path = string.Format(NANOLEAF_AUTHTOKEN, ip);
            var setting = sm.RegisteredSettings.FirstOrDefault(s => s.Path.Equals(path));
            if (setting == null)
                addControllerAsync(ip);
            else
                addControllerAsync(ip, sm.getSetting<string>(ESettingsType.APPLICATION, path));
        }
        private async Task addControllerAsync(string ip, string authToken = null)
        {
            try
            {
                Controller controller = new Controller(ip, authToken);
                if (clients.Any(c => c.IP.Equals(controller.IP)))
                {
                    controller.SelfDestruction();
                    Log.Info(string.Format("Client already Connected!" + Environment.NewLine + "{0}", controller));
                    return;
                }
                controller.AuthTokenReceived += Controller_AuthTokenReceived;
                clients.Add(controller);
                await Task.Delay(100);
                Log.Info($"Controller Added: {controller.ToString()}");

                if (isStarted)
                    bindInputAssignment();
            }
            catch (Exception e)
            {
                Log.Warn(string.Empty, e);
            }
        }

        private void Controller_AuthTokenReceived(object sender, EventArgs e)
        {
            SettingsManager sm = SettingsManager.getInstance();
            string path = string.Format(NANOLEAF_AUTHTOKEN, ((Controller)sender).IP);
            if (!sm.RegisteredSettings.Any(s => s.Path.Equals(path)))
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, string.Format(T._("Auth. Token {0}"), ((Controller)sender).IP), path, String.Empty), ((Controller)sender).Auth_token);
            else
                sm.setSetting(ESettingsType.APPLICATION, path, ((Controller)sender).Auth_token);
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
        private async Task debindInputAssignment()
        {
            try
            {
                var im = InputManager.getInstance();
                var sinks = im.RegisteredSinks.Where(s => s.Category.Name.Equals("Nanoleaf"));
                var sources = im.RegisteredSources.Where(s => s.Category.Name.Equals("Nanoleaf"));
                im.UnregisterSinks(sinks);
                im.UnregisterSources(sources);
                Log.Info("Unregisterd {0} Sinks and {1} Sources", sinks.Count(), sources.Count());
            }
            catch(Exception e)
            {
                Log.Error(string.Empty, e);
            }
        }
        protected override void initializePlugin()
        {
        }

        protected override void shutdownPlugin()
        {
            SettingsManager sm = SettingsManager.getInstance();
            Communication.StopDiscoveryTask();
            clients.ForEach(c => c.SelfDestruction());
            clients.Clear();
            Log.Info("Shutdown");
            debindInputAssignment();
            isStarted = false;
        }

        protected override void startupPlugin()
        {
            Log.Info("Start");
            try
            {

                SettingsManager sm = SettingsManager.getInstance();
                if (!sm.RegisteredSettings.Any(s => s.Path.Equals(NANOLEAF_SHOW_IN_INPUTASSIGNMENT)))
                {
                    sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Show in InputAssignment"), NANOLEAF_SHOW_IN_INPUTASSIGNMENT, String.Empty), true);
                    sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Known Controler IPs"), NANOLEAF_KNOWN_CONTROLER_IPS, T._("Tipe here the IP adresses of your Controllers.") + Environment.NewLine + T._("Example:") + Environment.NewLine + "192.168.10.33; 192.168.10.35; ...; 192.168.10.42"), "192.168.1.123");


                    ips = sm.getSetting<string>(ESettingsType.APPLICATION, NANOLEAF_KNOWN_CONTROLER_IPS);
                    foreach (string ip in ips.Replace(" ", "").Split(';'))
                    {
                        string path = string.Format(NANOLEAF_AUTHTOKEN, ip);
                        if (!sm.RegisteredSettings.Any(s => s.Path.Equals(path)))
                            sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, string.Format(T._("Auth. Token {0}"), ip), path, String.Empty), "");
                    }
                }

                ips = sm.getSetting<string>(ESettingsType.APPLICATION, NANOLEAF_KNOWN_CONTROLER_IPS);
                if (!string.IsNullOrWhiteSpace(ips))
                    foreach (string ip in ips.Replace(" ", "").Split(';'))
                    {
                        string path = string.Format(NANOLEAF_AUTHTOKEN, ip);
                        var setting = sm.RegisteredSettings.FirstOrDefault(s => s.Path.Equals(path));
                        if (setting == null)
                            addControllerAsync(ip);
                        else
                            addControllerAsync(ip, sm.getSetting<string>(ESettingsType.APPLICATION, path));
                    }

                sm.SettingChanged += SettingChanged;

                Communication.IPs = KernelNetManager.getInstance().IPAddresses.Select(s=> IPAddress.Parse(s)).ToArray();
                Communication.StartDiscoveryTask();
                bindInputAssignment().GetAwaiter();
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
            }
            isStarted = true;
        }

        private void SettingChanged(object sender, SettingChangedEventArgs args)
        {
            switch (args.SettingsPath)
            {
                case NANOLEAF_SHOW_IN_INPUTASSIGNMENT:
                    if ((bool)args.NewValue)
                        bindInputAssignment();
                    else
                        debindInputAssignment();
                    break;
                case NANOLEAF_KNOWN_CONTROLER_IPS:
                    ips = (string)args.NewValue;
                    break;
            }
        }

        protected override void DisposePlugin(bool disposing)
        {
            debindInputAssignment();
            base.DisposePlugin(disposing);
            isDisposed = true;
        }
    }
}
