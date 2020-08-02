using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nanoleaf_Plugin.API
{
    public class Controller : IDisposable
    {
        public string IP { get; private set; }
        private const string PORT = "16021";
        public string Auth_token { get; private set; } = null;
        private bool isDisposed = false;

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
                Communication.SetPanelLayoutGlobalOrientation(IP, PORT, Auth_token, value);
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
        public void SetPowerOn()
        {
            Communication.SetStateOnOff(IP, PORT, Auth_token, true);
        }
        public void SetPowerOff()
        {
            Communication.SetStateOnOff(IP, PORT, Auth_token, false);
        }

        private ushort brightness;
        public ushort Brightness
        {
            get { return brightness; }
            set
            {
                Communication.SetStateBrightness(IP, PORT, Auth_token, value);
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
                Communication.SetStateHue(IP, PORT, Auth_token, value);
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
                Communication.SetStateSaturation(IP, PORT, Auth_token, value);
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
                Communication.SetStateColorTemperature(IP, PORT, Auth_token, value);
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
        public ReadOnlyCollection<Panel> Panels
        {
            get { return panels.AsReadOnly(); }
        }

        public event EventHandler NewPanelAdded;
        public event EventHandler PanelLayoutChanged;

        private ExternalControlConnectionInfo externalControlInfo;


        public Controller(string ip, string auth_token = null)
        {
            IP = ip;
            if (auth_token == null)
            {
                Task.Run(async () =>
                {
                    while (Auth_token == null)
                        try
                        {
                            Auth_token = await Communication.AddUser(ip, PORT);
                        }
                        catch (Exception e)
                        {
                            NanoleafPlugin.Log.Info($"Device({ip}) is maybe not in Pairing-Mode. Please Hold the Powerbutton til you see a Visual Feedback on the Controller (5-7)s");
                            await Task.Delay(8000);// If the Device is not in Pairing-Mode it tock 5-7s tu enable the Pairing mode by hand. We try it again abfer 8s
                        }
                });
            }
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

        ~Controller()
        {
            Dispose();
        }

        private async void runController()
        {
            while (!isDisposed && Auth_token == null)
                await Task.Delay(1000);

            UpdateInfos(await Communication.GetAllPanelInfo(IP, PORT, Auth_token));
            externalControlInfo = await Communication.SetExternalControlStreaming(IP, PORT, Auth_token, DeviceType);
            Communication.StaticOnLayoutEvent += Communication_StaticOnLayoutEvent;
            Communication.RegisterEventListener(IP, PORT, Auth_token);
            while (!isDisposed)
            {
                try
                {
                    UpdateInfos(await Communication.GetAllPanelInfo(IP, PORT, Auth_token));
                    await Task.Delay(5000);
                }
                catch (Exception e)
                {
                    NanoleafPlugin.Log.ErrorOrDebug(string.Empty, e);
                }
            }
        }

        private void UpdateInfos(AllPanelInfo allPanelInfo)
        {
            if (allPanelInfo == null)
                return;

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
                    panels.Add(new Panel(IP, pp, (ushort)layout.SideLength));
                    NewPanelAdded?.Invoke(null, EventArgs.Empty);
                }
            }

            PanelLayoutChanged?.Invoke(null, EventArgs.Empty);
        }

        private async void streamController()
        {
            while (!isDisposed && Auth_token == null)
                await Task.Delay(1000);

            while (!isDisposed)
            {
                Thread.Sleep(20);
                if (externalControlInfo != null)
                    Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(panels));
            }
        }

        public void SelfDestruction()
        {
            Dispose();
            Communication.DeleteUser(IP, PORT, Auth_token).GetAwaiter().GetResult();
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
