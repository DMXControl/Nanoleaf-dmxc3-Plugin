using LumosLIB.Kernel.Log;
using LumosLIB.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static Nanoleaf_Plugin.API.Panel;

namespace Nanoleaf_Plugin.API
{
    public class Controller : IDisposable
    {
        private static ILumosLog log = NanoleafPlugin.Log;
        public string IP { get; private set; }
        private const string PORT = "16021";

        public string Auth_token { get; private set; } = null;
        private bool isDisposed = false;
        private Ping ping = new Ping();

        public string Name { get; private set; }
        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public string SerialNumber { get; private set; }
        public string HardwareVersion { get; private set; }
        public string FirmwareVersion { get; private set; }

        public EDeviceType DeviceType { get; private set; }


        public uint NumberOfPanels { get; private set; }
        private ushort globalOrientation;
        public ushort GlobalOrientation
        {
            get { return globalOrientation; }
            private set
            {
                _ = Communication.SetPanelLayoutGlobalOrientation(IP, PORT, Auth_token, value);
            }
        }
        public ushort GlobalOrientationMin
        {
            get;
            private set;
        }
        public ushort GlobalOrientationMax
        {
            get;
            private set;
        }

        public string[] EffectList { get; private set; }
        public string SelectedEffect { get; private set; }

        public bool PowerOn { get; private set; }
        public bool PowerOff { get; private set; }

        private bool reachable;
        public bool Reachable
        {
            get
            {
                return this.reachable;
            }
            private set
            {
                if (this.reachable == value)
                    return;
                this.reachable = value;
                log.Info($"{this} is reachable.");
                this.establishConnection();
            }
        }
        public void SetPowerOn()
        {
            _ = Communication.SetStateOnOff(IP, PORT, Auth_token, true);
        }
        public void SetPowerOff()
        {
            _ = Communication.SetStateOnOff(IP, PORT, Auth_token, false);
        }

        private ushort brightness;
        public ushort Brightness
        {
            get { return brightness; }
            set
            {
                _ = Communication.SetStateBrightness(IP, PORT, Auth_token, value);
            }
        }
        public ushort BrightnessMin
        {
            get;
            private set;
        }
        public ushort BrightnessMax
        {
            get;
            private set;
        }
        private ushort hue;
        public ushort Hue
        {
            get { return hue; }
            set
            {
                _ = Communication.SetStateHue(IP, PORT, Auth_token, value);
            }
        }
        public ushort HueMin
        {
            get;
            private set;
        }
        public ushort HueMax
        {
            get;
            private set;
        }
        private ushort saturation;
        public ushort Saturation
        {
            get { return saturation; }
            set
            {
                _ = Communication.SetStateSaturation(IP, PORT, Auth_token, value);
            }
        }
        public ushort SaturationMin
        {
            get;
            private set;
        }
        public ushort SaturationMax
        {
            get;
            private set;
        }
        private ushort colorTemprature;
        public ushort ColorTemprature
        {
            get { return colorTemprature; }
            set
            {
                _ = Communication.SetStateColorTemperature(IP, PORT, Auth_token, value);
            }
        }
        public ushort ColorTempratureMin
        {
            get;
            private set;
        }
        public ushort ColorTempratureMax
        {
            get;
            private set;
        }
        public string ColorMode { get; private set; }
        private List<Panel> panels = new List<Panel>();
        private List<Panel> changedPanels = new List<Panel>();
        public ReadOnlyCollection<Panel> Panels
        {
            get { return panels.AsReadOnly(); }
        }

        public event EventHandler PanelAdded;
        public event EventHandler PanelRemoved;
        public event EventHandler PanelLayoutChanged;
        public event EventHandler AuthTokenReceived;
        public event EventHandler UpdatedInfos;

        private ExternalControlConnectionInfo externalControlInfo;

        public Controller(JToken json)
        {
            IP = (string)json[nameof(IP)];
            Auth_token = (string)json[nameof(Auth_token)];
            Name = (string)json[nameof(Name)];
            Model = (string)json[nameof(Model)];
            Manufacturer = (string)json[nameof(Manufacturer)];
            SerialNumber = (string)json[nameof(SerialNumber)];
            HardwareVersion = (string)json[nameof(HardwareVersion)];
            FirmwareVersion = (string)json[nameof(FirmwareVersion)];

            switch (Model)
            {
                case "NL22":
                    DeviceType = EDeviceType.LightPanles;
                    break;
                case "NL29":
                    DeviceType = EDeviceType.Canvas;
                    break;
                case "NL42":
                    DeviceType = EDeviceType.Shapes;
                    break;
                case "NL45":
                    DeviceType = EDeviceType.Essentials;
                    break;
                case "NL52":
                    DeviceType = EDeviceType.Elements;
                    break;
                case "NL59":
                    DeviceType = EDeviceType.Lines;
                    break;
            }

            NumberOfPanels = (uint)json[nameof(NumberOfPanels)];
            globalOrientation = (ushort)json[nameof(GlobalOrientation)];
            GlobalOrientationMin = (ushort)json[nameof(GlobalOrientationMin)];
            GlobalOrientationMax = (ushort)json[nameof(GlobalOrientationMax)];

            //EffectList = (string[])json[nameof(EffectList)].Select(c=>c.)
            SelectedEffect = (string)json[nameof(SelectedEffect)];
            PowerOn = (bool)json[nameof(PowerOn)];
            PowerOff = (bool)json[nameof(PowerOff)];

            brightness = (ushort)json[nameof(Brightness)];
            BrightnessMin = (ushort)json[nameof(BrightnessMin)];
            BrightnessMax = (ushort)json[nameof(BrightnessMax)];
            hue = (ushort)json[nameof(Hue)];
            HueMin = (ushort)json[nameof(HueMin)];
            HueMax = (ushort)json[nameof(HueMax)];
            saturation = (ushort)json[nameof(Saturation)];
            SaturationMin = (ushort)json[nameof(SaturationMin)];
            SaturationMax = (ushort)json[nameof(SaturationMax)];
            colorTemprature = (ushort)json[nameof(ColorTemprature)];
            ColorTempratureMin = (ushort)json[nameof(ColorTempratureMin)];
            ColorTempratureMax = (ushort)json[nameof(ColorTempratureMax)];
            ColorMode = (string)json[nameof(ColorMode)];

            var panels = json[nameof(Panels)];
            foreach (var p in panels)
                this.panels.Add(new Panel(p));

            startServices();
        }
        public Controller(string ip, string auth_token = null)
        {
            IP = ip;
            Auth_token = auth_token;
            if (Auth_token == null && NanoleafPlugin.AutoRequestToken)
            {
                RequestToken();
            }
            startServices();
        }

        ~Controller()
        {
            Dispose();
        }

        private void startServices()
        {
            Task taskRun = new Task(() =>
            {
                runController();
            });
            taskRun.Start();
            Thread threadStream = new Thread(() =>
            {
                streamController();
            });
            threadStream.IsBackground = true;
            threadStream.Priority = ThreadPriority.AboveNormal;
            threadStream.ApartmentState = ApartmentState.MTA;
            threadStream.Start();
        }

        internal void RequestToken(int tryes=20)
        {
            int count = 0;
            Task.Run(async () =>
            {
                while (Auth_token == null && !this.isDisposed)
                    try
                    {
                        Auth_token = await Communication.AddUser(IP, PORT);
                    }
                    catch (Exception e)
                    {
                        log.Info($"Device({IP}) is maybe not in Pairing-Mode. Please Hold the Powerbutton til you see a Visual Feedback on the Controller (5-7)s");
                        await Task.Delay(8000);// If the Device is not in Pairing-Mode it tock 5-7s tu enable the Pairing mode by hand. We try it again abfer 8s
                        count++;
                        if (count >= tryes && Auth_token == null)
                        {
                            log.Info($"Device({IP}) not Response after {count} tryes");
                            return;
                        }
                    }

                if (Auth_token != null)
                {
                    log.Info($"Received AuthToken ({Auth_token}) from Device({IP}) after {count} tryes");
                    AuthTokenReceived?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        private async void runController()
        {
            while (!isDisposed && Auth_token == null)
                await Task.Delay(1000);

            do
            {
                try
                {
                    var res = await ping.SendPingAsync(IP);
                    if (res.Status == IPStatus.Success)
                        this.Reachable = true;
                    else
                        this.Reachable = false;
                    await Task.Delay(5000);
                    if(this.Reachable && !isDisposed)
                        UpdateInfos(await Communication.GetAllPanelInfo(IP, PORT, Auth_token));
                }
                catch (Exception e)
                {
                    NanoleafPlugin.Log.ErrorOrDebug(string.Empty, e);
                }
            } while (!isDisposed);
        }
        private async void establishConnection()
        {
            try
            {
                Communication.StaticOnLayoutEvent -= Communication_StaticOnLayoutEvent;
                Communication.StaticOnLayoutEvent += Communication_StaticOnLayoutEvent;
                UpdateInfos(await Communication.GetAllPanelInfo(IP, PORT, Auth_token));
                await Communication.StartEventListener(IP, PORT, Auth_token);
                externalControlInfo = await Communication.SetExternalControlStreaming(IP, PORT, Auth_token, DeviceType);
            }
            catch (Exception e)
            {
                NanoleafPlugin.Log.ErrorOrDebug(string.Empty, e);
            }
        }

        private void UpdateInfos(AllPanelInfo allPanelInfo)
        {
            if (allPanelInfo == null)
            {
                this.Reachable = false;
                return;
            }
            this.Reachable = true;

            Name = allPanelInfo.Name;
            Model = allPanelInfo.Model;
            Manufacturer = allPanelInfo.Manufacturer;
            SerialNumber = allPanelInfo.SerialNumber;
            HardwareVersion = allPanelInfo.HardwareVersion;
            FirmwareVersion = allPanelInfo.FirmwareVersion;

            switch (Model)
            {
                case "NL22":
                    DeviceType = EDeviceType.LightPanles;
                    break;
                case "NL29":
                    DeviceType = EDeviceType.Canvas;
                    break;
                case "NL42":
                    DeviceType = EDeviceType.Shapes;
                    break;
                case "NL45":
                    DeviceType = EDeviceType.Essentials;
                    break;
                case "NL52":
                    DeviceType = EDeviceType.Elements;
                    break;
                case "NL59":
                    DeviceType = EDeviceType.Lines;
                    break;
            }

            NumberOfPanels = allPanelInfo.PanelLayout.Layout.NumberOfPanels;
            globalOrientation = allPanelInfo.PanelLayout.GlobalOrientation.Value;
            GlobalOrientationMin = (ushort)allPanelInfo.PanelLayout.GlobalOrientation.Min;
            GlobalOrientationMax = (ushort)allPanelInfo.PanelLayout.GlobalOrientation.Max;

            EffectList = allPanelInfo.Effects.List.ToArray();
            SelectedEffect = allPanelInfo.Effects.Selected;
            PowerOn = allPanelInfo.State.On.On;
            PowerOff = !PowerOn;

            brightness = allPanelInfo.State.Brightness.Value;
            BrightnessMin = (ushort)allPanelInfo.State.Brightness.Min;
            BrightnessMax = (ushort)allPanelInfo.State.Brightness.Max;
            hue = allPanelInfo.State.Hue.Value;
            HueMin = (ushort)allPanelInfo.State.Hue.Min;
            HueMax = (ushort)allPanelInfo.State.Hue.Max;
            saturation = allPanelInfo.State.Saturation.Value;
            SaturationMin = (ushort)allPanelInfo.State.Saturation.Min;
            SaturationMax = (ushort)allPanelInfo.State.Saturation.Max;
            colorTemprature = allPanelInfo.State.ColorTemprature.Value;
            ColorTempratureMin = (ushort)allPanelInfo.State.ColorTemprature.Min;
            ColorTempratureMax = (ushort)allPanelInfo.State.ColorTemprature.Max;
            ColorMode = allPanelInfo.State.ColorMode;

            UpdatedInfos?.Invoke(this, EventArgs.Empty);

            UpdatePanelLayout(allPanelInfo.PanelLayout.Layout);
        }

        private void Communication_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            if (!e.IP.Equals(IP))
                return;

            UpdatePanelLayout(e.LayoutEvent.Layout);
        }
        private void UpdatePanelLayout(Layout layout)
        {
            var ids = layout.PanelPositions.Select(p => p.PanelId);
            foreach (int id in ids)
            {
                if (!panels.Any(p => p.ID.Equals(id)))
                {
                    var pp = layout.PanelPositions.Single(p => p.PanelId.Equals(id));
                    panels.Add(new Panel(IP, pp));
                    PanelAdded?.Invoke(null, EventArgs.Empty);
                }
            }
            bool panelRemoved = false;
            panels.RemoveAll((p) => 
            { 
                bool remove = !ids.Any(id => id.Equals(p.ID));
                if (remove)
                    panelRemoved = true;
                return remove;
            });
            if(panelRemoved)
                PanelRemoved?.Invoke(null, EventArgs.Empty);

            PanelLayoutChanged?.Invoke(null, EventArgs.Empty);
        }

        private void streamController()
        {
            while (!isDisposed && Auth_token == null)
                Thread.Sleep(1000);

            long lastTimestamp = 0;
            long nowTimestamp = 0;
            int frameCounter = 0;
            while (!isDisposed)
            {
                nowTimestamp = DateTime.Now.Ticks;
                int refreshRate = NanoleafPlugin.RefreshRate.Limit(10, 60);
                double milliSinceLast = ((double)(nowTimestamp - lastTimestamp)) / TimeSpan.TicksPerMillisecond;
                double frameDuration = (1000 / refreshRate);
                if (milliSinceLast < frameDuration)
                {
                    if (milliSinceLast > frameDuration*2)
                        log.Warn($"Streaming-Thread last send {milliSinceLast}ms");
                    Thread.SpinWait(10);
                }
                else
                {
                    lastTimestamp = DateTime.Now.Ticks;
                    if (frameCounter >= refreshRate)//KeyFrame every 1s
                    {
                        lock (changedPanels)
                        {
                            changedPanels.Clear();
                            frameCounter = 0;
                            if (panels.NotEmpty())
                                Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(panels));
                        }
                    }
                    else if (externalControlInfo != null)//DeltaFrame
                    {
                        Panel[] _panels = new Panel[0];
                        lock (changedPanels)
                        {
                            if (!changedPanels.Empty())
                            {
                                _panels = changedPanels.ToArray();
                                changedPanels.Clear();
                            }
                        }
                        if (_panels.Length > 0)
                            Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(_panels));
                    }
                    frameCounter++;
                }
            }
        }


        public bool SetPanelColor(int panelID, RGBW color)
        {
            var panel = this.panels.FirstOrDefault(p => p.ID.Equals(panelID));
            if (panel != null)
            {
                panel.StreamingColor = color;
                lock (changedPanels)
                {
                    if (!changedPanels.Contains(panel))
                        changedPanels.Add(panel);
                }
                return true;
            }
            return false;
        }

        public void SelfDestruction(bool deleteUser=false)
        {
            Dispose();
            if (deleteUser)
                Communication.DeleteUser(IP, PORT, Auth_token).GetAwaiter().GetResult();

            log.Info(string.Format("Destruct {0}", this));
        }
        public void Dispose()
        {
            isDisposed = true;
        }

        public override string ToString()
        {
            return $"Name: {Name} IP: {IP} DeviceType: {DeviceType} SN: {SerialNumber}";
        }
    }
}
