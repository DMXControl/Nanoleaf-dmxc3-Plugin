﻿using LumosLIB.Kernel.Log;
using LumosLIB.Tools;
using LumosProtobuf.Resource;
using Microsoft.Extensions.Logging;
using Nanoleaf_Plugin.Plugin.Logging;
using Nanoleaf_Plugin.Plugin.MainSwitch;
using Nanoleaf_Plugin.Plugin.Sinks;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.HAL.Handler;
using org.dmxc.lumos.Kernel.Input.v2;
using org.dmxc.lumos.Kernel.MainSwitch;
using org.dmxc.lumos.Kernel.Plugin;
using org.dmxc.lumos.Kernel.Project;
using org.dmxc.lumos.Kernel.Resource;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;
using T = LumosLIB.Tools.I18n.DummyT;

namespace Nanoleaf_Plugin
{
    public class NanoleafPlugin : KernelPluginBase, IResourceProvider
    {
        internal static readonly ILumosLog Log = LumosLogger.getInstance(nameof(NanoleafPlugin));
        private static List<Controller> clients = new List<Controller>();

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
        internal const string NANOLEAF_REMOVE_CONTROLLER = "NANOLEAF.REMOVE_CONTROLLER";

        private bool isDisposed = false;
        private bool isStarted = false;

        public static event EventHandler ControllerAdded;

        internal static bool ShowInInputAssignment = true, Discover = true, AutoConnect = true, AutoRequestToken = true;

        private static int refreshRate = 44;
        internal static int RefreshRate
        {
            get => refreshRate;
            set
            {
                refreshRate = value;
                clients.ForEach(c => c.RefreshRate = value);
            }
        }

        private static string discoverState = "Stopped";
        public static string DiscoverState
        {
            get { return discoverState; }
            internal set
            {
                discoverState = value;
                sm?.SetKernelSetting(ESettingsType.APPLICATION, NANOLEAF_DISCOVER_STATE, value);
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
            foreach (var controller in clients.Where(c => c.DeviceType == deviceType || deviceType == EDeviceType.UNKNOWN))
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
            //Special LoggerWrapper is needed because Lumos uses the LumosLog class for logging, the NanoleafAPI uses MEL. 
            Tools.LoggerFactory = new LoggerFactory();
            Tools.LoggerFactory.AddProvider(new LumosLogWrapperProvider(nameof(NanoleafPlugin)));

            Communication.DeviceDiscovered += Communication_DeviceDiscovered;
        }

        private void Communication_DeviceDiscovered(object sender, DiscoveredEventArgs e)
        {
            Log.Info($"Device Discovered: {e.DiscoveredDevice.ToString()}");
            string ip = e.DiscoveredDevice.IP;
            string port = e.DiscoveredDevice.Port;
            if (string.Equals("6517", port))
            {
                Log?.Info($"Port is: {port}, falback to 16021");
                port = "16021";
            }
            string json = JsonSerializer.Serialize(Communication.DiscoveredDevices);
            sm.SetKernelSetting(ESettingsType.APPLICATION, NANOLEAF_DISCOVERED_CONTROLLERS, json);
            _ = addControllerAsync(Controller.CreateFromIPPort(ip, port));
        }
        private async Task addControllerAsync(Controller controller, bool setSettings = true)
        {
            try
            {
                if (clients.Any(c => c.IP.Equals(controller.IP)))
                {
                    Log.Info(string.Format("Controller already Connected! IP: {0}", controller.IP));
                    controller.Dispose();
                    return;
                }
                if (!(controller.IsInitializing || controller.IsInitialized))
                    await controller.Initialize();
                controller.AuthTokenReceived += Controller_AuthTokenReceived;
                controller.UpdatedInfos += Controller_UpdatedInfos;
                controller.PanelLayoutChanged += Controller_PanelLayoutChanged;
                clients.Add(controller);
                ControllerAdded?.Invoke(this, EventArgs.Empty);
                if (NanoleafMainSwitch.getInstance().Enabled)
                    _ = Task.Run(async () =>
                    {
                        for (int i = 0; i < 100; i++)
                            if (!controller.IsInitialized)
                                await Task.Delay(5000);
                        controller?.StartStreaming();
                    });

                if (setSettings)
                    saveClients();

                await Task.Delay(100);
                Log.Info($"Controller Added: {controller.ToString()}");

                if (ShowInInputAssignment)
                    _ = bindInputAssignment();
            }
            catch (Exception e)
            {
                Log.Warn(string.Empty, e);
            }
        }

        private async Task removeControllerAsync(Controller controller)
        {
            try
            {
                if (controller == null)
                    return;

                var controllerVita = controller.ToString();

                _ = debindControllerInputAssignment(controller);

                controller.AuthTokenReceived -= Controller_AuthTokenReceived;
                controller.UpdatedInfos -= Controller_UpdatedInfos;
                controller.PanelLayoutChanged -= Controller_PanelLayoutChanged;

                await Communication.DeleteUser(controller.IP, controller.Port, controller.Auth_token);

                controller.Dispose();
                clients.Remove(controller);

                saveClients();
                sm.SetKernelSetting(ESettingsType.APPLICATION, NANOLEAF_REMOVE_CONTROLLER, "");

                await Task.Delay(100);
                Log.Info("Controller removed: {0}", controllerVita);
            }
            catch (Exception e)
            {
                Log.Warn(string.Empty, e);
            }
        }


        private static void saveClients()
        {
            string json = JsonSerializer.Serialize(clients.Where(c => Tools.IsTokenValid(c.Auth_token) && c.DeviceType != EDeviceType.UNKNOWN));
            sm.SetKernelSetting(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS, json);
        }

        private void Controller_PanelLayoutChanged(object sender, EventArgs e)
        {
            saveClients();
            _ = bindInputAssignment();
        }

        private void Controller_UpdatedInfos(object sender, EventArgs e)
        {
            saveClients();
            _ = bindInputAssignment();
        }

        private void Controller_AuthTokenReceived(object sender, EventArgs e)
        {
            saveClients();
        }

        private bool bindInputAssignmentRunning = false;
        private async Task bindInputAssignment()
        {
            if (bindInputAssignmentRunning)
                return;
            bindInputAssignmentRunning = true;
            Controller[] list = null;
            list = clients.ToArray();
            if (list.Empty())
                return;
            foreach (var controller in list)
                try
                {
                    while (controller.SerialNumber == null)
                        await Task.Delay(1000);

                    var im = InputManager.getInstance();
                    if (!im.Sinks.Any(s => s.ID.Contains(controller.SerialNumber)))
                    {
                        try
                        {
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

                            im.RegisterSink(new BrightnessSink(controller.SerialNumber));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Empty, ex);
                        }
                    }

                    foreach (Panel panel in controller.Panels)
                    {
                        var panelSources = im.Sinks.OfType<CanvasSink>().Where(s => s.ID.Contains(panel.ID.ToString())).ToList();
                        if (!panelSources.Empty())
                            continue;
                        try
                        {
                            im.RegisterSource(CanvasPositionSource.CreateX(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasPositionSource.CreateY(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasPositionSource.CreateOrientation(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasTouchSource.CreateHover(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasTouchSource.CreateDown(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasTouchSource.CreateHold(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasTouchSource.CreateUp(controller.SerialNumber, panel.ID));
                            im.RegisterSource(CanvasTouchSource.CreateSwipe(controller.SerialNumber, panel.ID));

                            var sink = new CanvasSink(controller.SerialNumber, panel);
                            if (!im.Sinks.OfType<CanvasSink>().Any(s => s.ID.Equals(sink.ID)))
                                im.RegisterSink(sink);

                            im.RegisterSink(new CanvasSink(controller.SerialNumber, panel, true));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Empty, ex);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(string.Empty, e);
                }

            bindInputAssignmentRunning = false;
        }

        private async Task debindInputAssignment()
        {
            try
            {
                var im = InputManager.getInstance();
                var sinks = im.Sinks.Where(s => s.Category.Name.Equals("Nanoleaf"));
                var sources = im.Sources.Where(s => s.Category.Name.Equals("Nanoleaf"));
                im.UnregisterSinks(sinks);
                im.UnregisterSources(sources);
                Log.Info("Unregisterd {0} Sinks and {1} Sources", sinks.Count(), sources.Count());
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
            }
        }

        private async Task debindControllerInputAssignment(Controller controller)
        {
            try
            {
                var im = InputManager.getInstance();
                var sinks = im.Sinks.Where(s => s.ID.Equals("Nanoleaf-" + controller.SerialNumber));
                var sources = im.Sources.Where(s => s.ID.Equals("Nanoleaf-" + controller.SerialNumber));
                im.UnregisterSinks(sinks);
                im.UnregisterSources(sources);
                Log.Info("Unregisterd {0} Sinks and {1} Sources of Controller {2}", sinks.Count(), sources.Count(), controller.SerialNumber);
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
            }
        }

        protected override void initializePlugin()
        {
            Log.Info("Initialize");
            sm = SettingsManager.getInstance();
            ResourceManager.getInstance().registerResourceProvider(this);
            HandlerFactory.getInstance().registerHandlerNode<NanoleafHandlerNode>("nanoleaf");
            HandlerFactory.getInstance().registerHandlerNode<NanoleafControllerHandlerNode>("nanoleaf-controller");
            DeviceManager.getInstance().registerDeviceFactory(new NanoleafDeviceFactory());

            NanoleafMainSwitch.getInstance().EnabledChanged += NanoleafMainSwitch_EnabledChanged;
            MainSwitchManager.getInstance().RegisterMainSwitch(NanoleafMainSwitch.getInstance());
            Log.Info("Version: {0}, NanolaefAPI Version: {1}", Assembly.GetExecutingAssembly().GetName().Version, Assembly.GetAssembly(typeof(Communication)).GetName().Version);
        }

        private void NanoleafMainSwitch_EnabledChanged(object sender, EventArgs e)
        {
            if (NanoleafMainSwitch.getInstance().Enabled)
            {
                clients.ForEach(client =>
                {
                    _ = client.StartStreaming();
                });
            }
            else
            {
                clients.ForEach(client =>
                {
                    _ = client.StopStreaming();
                });
            }
        }

        protected override void shutdownPlugin()
        {
            Communication.StopDiscoverySSDPTask();
            Communication.StopDiscoverymDNSTask();
            clients.ForEach(c => c.SelfDestruction());

            try
            {
                Communication.StopEventListener();
            }
            catch (Exception)
            {
                //Needed to prevent exception when stopping the Nanoleaf plugin
            }
            clients.Clear();
            Log.Info("Shutdown");
            _ = debindInputAssignment();
            isStarted = false;
        }

        protected override void startupPlugin()
        {
            Log.Info("Start");
            try
            {
                registerSettings();
                sm.SettingChanged += SettingChanged;


                ShowInInputAssignment = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_SHOW_IN_INPUTASSIGNMENT);
                Discover = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_DISCOVER);
                AutoRequestToken = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_AUTOREQUEST_TOKEN);
                AutoConnect = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NANOLEAF_AUTOCONNECT);
                RefreshRate = sm.GetKernelSetting<int>(ESettingsType.APPLICATION, NANOLEAF_REFRESH_RATE);

                if (AutoConnect)
                {
                    string jsonControllers = sm.GetKernelSetting<string>(ESettingsType.APPLICATION, NANOLEAF_CONTROLLERS);
                    if (!string.IsNullOrWhiteSpace(jsonControllers))
                    {
                        var objControllers = JsonSerializer.Deserialize<IReadOnlyList<Controller>>(jsonControllers);
                        if (objControllers != null)
                            foreach (Controller controller in objControllers)
                                _ = addControllerAsync(controller, false);
                    }
                }


                Communication.RegisterIPAddress(IPAddress.Any);// KernelNetManager.getInstance().IPAddresses.Select(s=> IPAddress.Parse(s)).ToArray();
                if (Discover)
                {
                    try
                    {
                        Communication.StartDiscoverymDNSTask();
                        Communication.StartDiscoverySSDPTask();
                    }
                    catch (Exception e)
                    {
                        Log.ErrorOrDebug(e);
                    }
                }

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
            Controller controller;
            switch (args.SettingsPath)
            {
                case NANOLEAF_SHOW_IN_INPUTASSIGNMENT:
                    ShowInInputAssignment = (bool)args.NewValue;
                    if (ShowInInputAssignment)
                        _ = bindInputAssignment();
                    else
                        _ = debindInputAssignment();
                    break;

                case NANOLEAF_DISCOVER:
                    Discover = (bool)args.NewValue;
                    try
                    {
                        if (Discover)
                        {
                            Communication.StartDiscoverymDNSTask();
                            Communication.StartDiscoverySSDPTask();
                        }
                        else
                        {
                            Communication.StartDiscoverySSDPTask();
                            Communication.StopDiscoverymDNSTask();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.ErrorOrDebug(e);
                    }
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
                    controller = clients.FirstOrDefault(c => ip.Equals(c.IP));
                    if (controller != null)
                        controller.RequestToken();
                    break;

                case NANOLEAF_ADD_CONTROLLER:
                    string jsonController = (string)args.NewValue;
                    if (string.IsNullOrWhiteSpace(jsonController))
                        break;
                    Controller objController = JsonSerializer.Deserialize<Controller>(jsonController);
                    _ = addControllerAsync(objController);
                    break;
                case NANOLEAF_REMOVE_CONTROLLER:
                    ip = (string)args.NewValue;
                    if (string.IsNullOrWhiteSpace(ip))
                        break;
                    controller = clients.FirstOrDefault(c => ip.Equals(c.IP));

                    if (controller == null)
                    {
                        Log.Info(string.Format("Controller with IP {0} could not be found", ip));
                        return;
                    }

                    _ = removeControllerAsync(controller);
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
            _ = debindInputAssignment();
            base.DisposePlugin(disposing);
            isDisposed = true;
        }

        private void registerSettings()
        {
            if (!sm.RegisteredKernelSettings.Any(s => s.Path.Equals(NANOLEAF_SHOW_IN_INPUTASSIGNMENT)))
            {
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Show in InputAssignment"), NANOLEAF_SHOW_IN_INPUTASSIGNMENT, String.Empty), ShowInInputAssignment);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discover"), NANOLEAF_DISCOVER, String.Empty), Discover);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Autorequest Token"), NANOLEAF_AUTOREQUEST_TOKEN, String.Empty), AutoRequestToken);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Autoconnect"), NANOLEAF_AUTOCONNECT, String.Empty), AutoConnect);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Refresh Rate"), NANOLEAF_REFRESH_RATE, String.Empty), RefreshRate);

                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discover State"), NANOLEAF_DISCOVER_STATE, String.Empty), DiscoverState);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Discovered Controllers"), NANOLEAF_DISCOVERED_CONTROLLERS, String.Empty), string.Empty);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Controllers"), NANOLEAF_CONTROLLERS, String.Empty), string.Empty);

                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Request Token"), NANOLEAF_REQUEST_TOKEN, String.Empty), string.Empty);
                sm.RegisterKernelSetting(new SettingsMetadata(ESettingsRegisterType.APPLICATION, SETTINGS_CATEGORY_ID, T._("Add Controller"), NANOLEAF_ADD_CONTROLLER, String.Empty), string.Empty);
            }
        }


        public bool existsResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.DeviceImage)
            {
                if (name.Equals("Canvas")
                    || name.Equals("LightPanles")
                    || name.Equals("Shapes")
                    || name.Equals("Shapes-Black")
                    || name.Equals("Elements")
                    || name.Equals("Lines")
                    || name.Equals("Essentials"))
                    return true;
            }
            return false;
        }

        IReadOnlyList<LumosDataMetadata> IResourceProvider.allResources(EResourceDataType type)
        {
            if (type == EResourceDataType.DeviceImage)
            {
                List<LumosDataMetadata> ret = new List<LumosDataMetadata>()
                {
                    new LumosDataMetadata("Canvas"),
                    new LumosDataMetadata("LightPanles"),
                    new LumosDataMetadata("Shapes"),
                    new LumosDataMetadata("Shapes-Black"),
                    new LumosDataMetadata("Elements"),
                    new LumosDataMetadata("Lines"),
                    new LumosDataMetadata("Essentials"),
                };
                return ret.AsReadOnly();
            }

            return null;
        }

        [SupportedOSPlatform("windows")]
        public Stream loadResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.DeviceImage)
            {
                if (name.Equals("Canvas"))
                    return toByteArray(Properties.Resources.NanoleafCanvas);

                else if (name.Equals("LightPanles"))
                    return toByteArray(Properties.Resources.NanoleafLightPanles);

                else if (name.Equals("Shapes"))
                    return toByteArray(Properties.Resources.NanoleafShapes);
                else if (name.Equals("Shapes-Black"))
                    return toByteArray(Properties.Resources.NanoleafShapesBlack);

                else if (name.Equals("Elements"))
                    return toByteArray(Properties.Resources.NanoleafElements);

                else if (name.Equals("Lines"))
                    return toByteArray(Properties.Resources.NanoleafLines);

                else if (name.Equals("Essentials"))
                    return toByteArray(Properties.Resources.NanoleafEssentials);
            }

            return null;
        }

        [SupportedOSPlatform("windows")]
        private Stream toByteArray(Bitmap i)
        {
            var m = new System.IO.MemoryStream();
            if (i != null)
            {
                i.Save(m, ImageFormat.Png);
                m.Position = 0;
                return m;
            }
            return null;
        }
    }
}
