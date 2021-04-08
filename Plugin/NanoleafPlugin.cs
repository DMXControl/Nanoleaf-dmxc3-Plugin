using LumosLIB.Kernel.Log;
using Nanoleaf_Plugin.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.dmxc.lumos.Kernel.HAL.Handler;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.Net;
using org.dmxc.lumos.Kernel.Plugin;
using org.dmxc.lumos.Kernel.Project;
using org.dmxc.lumos.Kernel.Resource;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using T = LumosLIB.Tools.I18n.DummyT;

namespace Nanoleaf_Plugin
{
    public class NanoleafPlugin : KernelPluginBase, IResourceProvider
    {
        internal static readonly ILumosLog Log = LumosLogger.getInstance(nameof(NanoleafPlugin));
        private static List<Controller> clients = new List<Controller>();
        private static List<Controller> clientsBindedToInputAssignment = new List<Controller>();
        private const string SETTINGS_CATEGORY_ID = "Settings:Nanoleaf";

        internal const string NANOLEAF_SHOW_IN_INPUTASSIGNMENT = "NANOLEAF.SHOW_IN_INPUTASSIGNMENT";
        internal const string NANOLEAF_DISCOVER = "NANOLEAF.DISCOVER";
        internal const string NANOLEAF_AUTOREQUEST_TOKEN = "NANOLEAF.AUTOREQUEST_TOKEN";
        internal const string NANOLEAF_AUTOCONNECT = "NANOLEAF.AUTOCONNECT";
        internal const string NANOLEAF_REFRESH_RATE = "NANOLEAF.REFRESH_RATE";

        internal const string NANOLEAF_DISCOVER_STATE = "NANOLEAF.DISCOVER_STATE";
        internal const string NANOLEAF_DISCOVERED_CONTROLLERS = "NANOLEAF.DISCOVERED_CONTROLLERS";
        internal const string NANOLEAF_CONTROLLERS = "NANOLEAF.CONTROLLERS";

        internal const string NANOLEAF_REQUEST_TOKEN = "NANOLEAF.REQUEST_TOKEN";
        internal const string NANOLEAF_ADD_CONTROLLER = "NANOLEAF.ADD_CONTROLLER";

        private bool isDisposed = false;
        private bool isStarted = false;

        public static event EventHandler ControllerAdded;

        internal static bool ShowInInputAssignment = true, Discover = true, AutoConnect = true, AutoRequestToken = true;
        internal static int RefreshRate = 44;

        private static string discoverState = "Stopped";
        public static string DiscoverState
        {
            get { return discoverState; }
            internal set
            {
                discoverState = value;
                sm?.setSetting(ESettingsType.APPLICATION, NANOLEAF_DISCOVER_STATE, value);
            }
        }

        private static SettingsManager sm;
        internal static Controller getClient(string serialNumber)
        {
            if (serialNumber == null)
                return null;
            return clients?.FirstOrDefault(c => serialNumber.Equals(c.SerialNumber));
        }

        internal static IReadOnlyCollection<Panel> getAllPanels(EDeviceType deviceType)
        {
            List<Panel> panels = new List<Panel>();
            foreach (var controller in clients.Where(c => c.DeviceType == deviceType|| deviceType== EDeviceType.UNKNOWN))
                panels.AddRange(controller.Panels);
            return panels.AsReadOnly();
        }
        internal static Controller getControllerFromPanel(int id)
        {
            return clients?.FirstOrDefault(c => c.Panels.Any(p => p.ID.Equals(id)));
        }
        internal static IReadOnlyCollection<Controller> getControllers()
        {
            return clients.AsReadOnly();
        }
        public NanoleafPlugin() : base("{25a96576-fda7-4297-bc59-6c4f2256ab6e}", "Nanoleaf-Plugin")
        {
//#if DEBUG
//            while (!Debugger.IsAttached)
//            {
//                Thread.Sleep(100);
//            }
//            Log.Info("Debugger attaced!");
//#endif
            Communication.DeviceDiscovered += Communication_DeviceDiscovered;
        }

        private void Communication_DeviceDiscovered(object sender, DiscoveredEventArgs e)
        {
            Log.Info($"Device Discovered: {e.DiscoveredDevice.ToString()}");
            string ip = e.DiscoveredDevice.IP;
            string json= JsonConvert.SerializeObject(Communication.DiscoveredDevices);
            sm.setSetting(ESettingsType.APPLICATION, NANOLEAF_DISCOVERED_CONTROLLERS, json);
            addControllerAsync(ip);
        }
        private async Task addControllerAsync(string ip, string authToken = null, bool setSettings=true)
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
                controller.UpdatedInfos += Controller_UpdatedInfos;
                controller.PanelLayoutChanged += Controller_PanelLayoutChanged;
                clients.Add(controller);
                ControllerAdded?.Invoke(this, EventArgs.Empty);
                if (setSettings)
                {
                    string json = JsonConvert.SerializeObject(clients);
                    sm.setSetting(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS, json);
                }
                await Task.Delay(100);
                Log.Info($"Controller Added: {controller.ToString()}");

                bindInputAssignment();
            }
            catch (Exception e)
            {
                Log.Warn(string.Empty, e);
            }
        }

        private void Controller_PanelLayoutChanged(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(clients);
            sm.setSetting(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS, json);
        }

        private void Controller_UpdatedInfos(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(clients);
            sm.setSetting(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS, json);
        }

        private void Controller_AuthTokenReceived(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(clients);
            sm.setSetting(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS, json);
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
            sm = SettingsManager.getInstance();
            ResourceManager.getInstance().registerResourceProvider(this);
            HandlerFactory.getInstance().registerHandlerNode<NanoleafHandlerNode>("nanoleaf");
            DeviceManager.getInstance().registerDeviceFactory(new NanoleafDeviceFactory());
        }

        protected override void shutdownPlugin()
        {
            Communication.StopDiscoveryTask();
            Communication.StopEventListener();
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
                registerSettings();
                sm.SettingChanged += SettingChanged;


                ShowInInputAssignment = sm.getSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_SHOW_IN_INPUTASSIGNMENT);
                Discover = sm.getSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_DISCOVER);
                AutoRequestToken = sm.getSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_AUTOREQUEST_TOKEN);
                AutoConnect = sm.getSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_AUTOCONNECT);
                RefreshRate = sm.getSetting<int>(ESettingsType.APPLICATION, NANOLEAF_REFRESH_RATE);

                if (AutoConnect)
                {
                    string jsonControllers = sm.getSetting<string>(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS);
                    JArray objControllers = JsonConvert.DeserializeObject(jsonControllers) as JArray;
                    if (objControllers != null)
                        foreach (var c in objControllers)
                        {
                            var controller = new Controller(c);
                            clients.Add(controller);
                            controller.AuthTokenReceived += Controller_AuthTokenReceived;
                            controller.UpdatedInfos += Controller_UpdatedInfos;
                            controller.PanelLayoutChanged += Controller_PanelLayoutChanged;
                            ControllerAdded?.Invoke(this, EventArgs.Empty);
                        }
                }


                Communication.IPs = KernelNetManager.getInstance().IPAddresses.Select(s=> IPAddress.Parse(s)).ToArray();
                if(Discover)
                    Communication.StartDiscoveryTask();
                if (ShowInInputAssignment)
                    bindInputAssignment().GetAwaiter();

                Communication.StartEventListener();
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
            }
            isStarted = true;
        }

        private void SettingChanged(object sender, SettingChangedEventArgs args)
        {
            string ip = null;
            switch (args.SettingsPath)
            {
                case NANOLEAF_SHOW_IN_INPUTASSIGNMENT:
                    ShowInInputAssignment = (bool)args.NewValue;
                    if (ShowInInputAssignment)
                        bindInputAssignment();
                    else
                        debindInputAssignment();
                    break;

                case NANOLEAF_DISCOVER:
                    Discover = (bool)args.NewValue;
                    if (Discover)
                        Communication.StartDiscoveryTask();
                    else
                        Communication.StopDiscoveryTask();
                    break;

                case NANOLEAF_AUTOREQUEST_TOKEN:
                    AutoRequestToken = (bool)args.NewValue;
                    break;
                case NANOLEAF_AUTOCONNECT:
                    AutoConnect = (bool)args.NewValue;
                    break;

                case NANOLEAF_REFRESH_RATE:
                    RefreshRate = (int)args.NewValue;
                    break;

                case NANOLEAF_REQUEST_TOKEN:
                    ip = (string)args.NewValue;
                    if (string.IsNullOrWhiteSpace(ip))
                        break;
                    var controller = clients.FirstOrDefault(c => ip.Equals(c.IP));
                    controller.RequestToken();
                    break;

                case NANOLEAF_ADD_CONTROLLER:
                    string jsonController = (string)args.NewValue;
                    if (string.IsNullOrWhiteSpace(jsonController))
                        break;
                    JObject objController = JsonConvert.DeserializeObject(jsonController) as JObject;
                    ip = (string)objController["IP"];
                    string token = (string)objController["Token"];

                    if (objController["token"] != null)
                        addControllerAsync(ip, token);
                    else
                        addControllerAsync(ip);
                    break;
            }
        }

        protected override void DisposePlugin(bool disposing)
        {
            clients.ForEach(c => 
            {
                c.AuthTokenReceived -= Controller_AuthTokenReceived;
                c.UpdatedInfos -= Controller_UpdatedInfos;
                c.PanelLayoutChanged += Controller_PanelLayoutChanged;
                c.SelfDestruction();
            });
            debindInputAssignment();
            base.DisposePlugin(disposing);
            isDisposed = true;
        }

        private void registerSettings()
        {
            if (!sm.RegisteredSettings.Any(s => s.Path.Equals(NANOLEAF_SHOW_IN_INPUTASSIGNMENT)))
            {
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Show in InputAssignment"), NANOLEAF_SHOW_IN_INPUTASSIGNMENT, String.Empty), ShowInInputAssignment);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discover"), NANOLEAF_DISCOVER, String.Empty), Discover);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Autorequest Token"), NANOLEAF_AUTOREQUEST_TOKEN, String.Empty), AutoRequestToken);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Autoconnect"), NANOLEAF_AUTOCONNECT, String.Empty), AutoConnect);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Refresh Rate"), NANOLEAF_REFRESH_RATE, String.Empty), RefreshRate);

                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discover State"), NANOLEAF_DISCOVER_STATE, String.Empty), DiscoverState);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discovered Controllers"), NANOLEAF_DISCOVERED_CONTROLLERS, String.Empty), string.Empty);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Controllers"), NANOLEAF_CONTROLLERS, String.Empty), string.Empty);

                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Request Token"), NANOLEAF_REQUEST_TOKEN, String.Empty), string.Empty);
                sm.registerSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Add Controller"), NANOLEAF_ADD_CONTROLLER, String.Empty), string.Empty);
            }
        }


        public bool existsResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.DEVICE_IMAGE)
            {
                if (name.Equals(EDeviceType.Canvas.ToString())
                    || name.Equals(EDeviceType.LightPanles.ToString())
                    || name.Equals(EDeviceType.Shapes.ToString()))
                    return true;
            }
            return false;
        }
        public ReadOnlyCollection<LumosDataMetadata> allResources(EResourceDataType type)
        {
            if (type == EResourceDataType.DEVICE_IMAGE)
            {
                List<LumosDataMetadata> ret = new List<LumosDataMetadata>()
                {
                    new LumosDataMetadata(EDeviceType.Canvas.ToString()),
                    new LumosDataMetadata(EDeviceType.LightPanles.ToString()),
                    new LumosDataMetadata(EDeviceType.Shapes.ToString()),
                };
                return ret.AsReadOnly();
            }

            return null;
        }
        public byte[] loadResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.DEVICE_IMAGE)
            {
                if (name.Equals(EDeviceType.Canvas.ToString()))
                    return toByteArray(Properties.Resources.NanoleafCanvas);

                else if (name.Equals(EDeviceType.LightPanles.ToString()))
                    return toByteArray(Properties.Resources.NanoleafLightPanles);

                else if (name.Equals(EDeviceType.Shapes.ToString()))
                    return toByteArray(Properties.Resources.NanoleafShapes);
            }

            return null;
        }
        private byte[] toByteArray(Bitmap i)
        {
            using (var m = new System.IO.MemoryStream())
            {
                if (i != null)
                {

                    i.Save(m, ImageFormat.Png);
                    byte[] b = m.ToArray();
                    return b;
                }
            }
            return null;
        }
    }
}
