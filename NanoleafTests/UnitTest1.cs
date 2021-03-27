using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nanoleaf_Plugin.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Nanoleaf_Plugin.API.StateEvent;

namespace NanoleafTests
{
    [TestClass]
    public class UnitTest1
    {
        const string IP = "192.168.1.123";
        const string PORT = "16021";
        const string AUTH_TOKEN = "cGwULwjNBdgdxpjnFbMlUkJIcCPUXjpH";
        [TestMethod]
        public async Task TestAddUserAndDeleteUserAsync()
        {
            string authToken = await Communication.AddUser(IP, PORT);
            Assert.IsTrue(authToken != null);
            bool sucess = await Communication.DeleteUser(IP, PORT, authToken);
            Assert.IsTrue(sucess);
        }
        [TestMethod]
        public async Task TestManyGetMethodes()
        {
            await Task.Delay(500);
            var info = await Communication.GetAllPanelInfo(IP, PORT, AUTH_TOKEN);
            Assert.AreEqual("S19124C8036", info.SerialNumber);
            Assert.AreEqual("NL29", info.Model);
            Assert.AreEqual("Canvas C097", info.Name);
            Assert.AreEqual("2.0-4", info.HardwareVersion);
            Assert.AreEqual("Nanoleaf", info.Manufacturer);

            Assert.AreEqual(info.State.Brightness.Value, await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN));
            Assert.AreEqual(info.State.Saturation.Value, await Communication.GetStateSaturation(IP, PORT, AUTH_TOKEN));
            Assert.AreEqual(info.State.Hue.Value, await Communication.GetStateHue(IP, PORT, AUTH_TOKEN));
            Assert.AreEqual(info.State.ColorTemprature.Value, await Communication.GetStateColorTemperature(IP, PORT, AUTH_TOKEN));
            Assert.AreEqual(info.State.ColorMode, await Communication.GetColorMode(IP, PORT, AUTH_TOKEN));
            Assert.AreEqual(info.State.On.On, await Communication.GetStateOnOff(IP, PORT, AUTH_TOKEN));
        }
        [TestMethod]
        public async Task TestStreaming()
        {
            await Task.Delay(500);
            var info = await Communication.GetPanelLayoutLayout(IP, PORT, AUTH_TOKEN);

            var externalControlInfo = await Communication.SetExternalControlStreaming(IP, PORT, AUTH_TOKEN, EDeviceType.Canvas);

            List<Panel> panels = new List<Panel>();
            var ids = info.PanelPositions.Select(p => p.PanelId);
            foreach (int id in ids)
            {
                var pp = info.PanelPositions.Single(p => p.PanelId.Equals(id));
                panels.Add(new Panel(IP, pp, (ushort)info.SideLength));
            }

            var rgbw = new Panel.RGBW(0, 0, 0, 0);
            byte val = 0;
            do
            {
                rgbw = new Panel.RGBW(val, 0, 0, 0);
                panels.ForEach(p => p.StreamingColor = rgbw);
                Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(panels));
                if (val % 2 == 0)
                    Task.Delay(1).Wait();
                val++;
            }
            while (val != 0);

            do
            {
                rgbw = new Panel.RGBW(255, val, val, 0);
                var controlPanel = panels.Where(p => p.Shape == PanelPosition.EShapeType.ControlSquarePrimary).ToList();
                controlPanel.ForEach(p => p.StreamingColor = rgbw);
                Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(controlPanel));
                if (val % 2 == 0)
                    Task.Delay(1).Wait();
                val++;
            }
            while (val != 0);
            do
            {
                panels.ForEach(p => p.StreamingColor = new Panel.RGBW((byte)(p.StreamingColor.R - 1), (byte)(p.StreamingColor.G - 1), (byte)(p.StreamingColor.B - 1), 0));
                Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(panels));
                if (val % 2 == 0)
                    Task.Delay(1).Wait();
                val++;
            }
            while (panels.First().StreamingColor.R != 0);

            rgbw = new Panel.RGBW(0, 0, 0, 0);
            panels.ForEach(p => p.StreamingColor = rgbw);
            Communication.SendUDPCommand(externalControlInfo, Communication.CreateStreamingData(panels));
        }
        [TestMethod]
        public async Task TestDiscovery()
        {
            await Task.Delay(500);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool eventFired = false;
            Communication.DeviceDiscovered += (o, e) => { eventFired = true; };

            while (Communication.DiscoveredDevices.Count == 0)
            {
                if (sw.Elapsed.TotalSeconds > 180)
                    Assert.Fail("Discover should be done in max 1 minute!");
                else
                    await Task.Delay(1);
            }
            Assert.IsTrue(eventFired);
            Assert.AreEqual(EDeviceType.Canvas, Communication.DiscoveredDevices.First().DeviceTyp);
            Assert.AreEqual("Canvas C097", Communication.DiscoveredDevices.First().Name);
            Assert.AreEqual(IP, Communication.DiscoveredDevices.First().IP);
        }
        [TestMethod]
        public async Task TestGetSetEffects()
        {
            await Task.Delay(500);
            var list = await Communication.GetEffectList(IP, PORT, AUTH_TOKEN);
            Assert.IsNotNull(list);
            foreach (string effect in list)
            {
                Assert.IsTrue(await Communication.SetSelectedEffect(IP, PORT, AUTH_TOKEN, effect));
                var selectedEffect = await Communication.GetSelectedEffect(IP, PORT, AUTH_TOKEN);
                Assert.AreEqual(effect, selectedEffect);
            }
        }
        [TestMethod]
        public async Task TestGetSetColorTemperature()
        {
            await Task.Delay(500);
            var backupCT = await Communication.GetStateColorTemperature(IP, PORT, AUTH_TOKEN);
            Assert.IsTrue(backupCT != 0);
            Assert.IsTrue(await Communication.SetStateColorTemperature(IP, PORT, AUTH_TOKEN, 1200));
            Assert.AreEqual(1200, await Communication.GetStateColorTemperature(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateColorTemperatureIncrement(IP, PORT, AUTH_TOKEN, 5300));
            Assert.AreEqual(6500, await Communication.GetStateColorTemperature(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateColorTemperature(IP, PORT, AUTH_TOKEN, backupCT));
            Assert.AreEqual(backupCT, await Communication.GetStateColorTemperature(IP, PORT, AUTH_TOKEN));
        }
        [TestMethod]
        public async Task TestGetSetSaturation()
        {
            await Task.Delay(500);
            var backupSat = await Communication.GetStateSaturation(IP, PORT, AUTH_TOKEN);;
            Assert.IsTrue(await Communication.SetStateSaturation(IP, PORT, AUTH_TOKEN, 10));
            Assert.AreEqual(10, await Communication.GetStateSaturation(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateSaturationIncrement(IP, PORT, AUTH_TOKEN, 90));
            Assert.AreEqual(100, await Communication.GetStateSaturation(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateSaturation(IP, PORT, AUTH_TOKEN, backupSat));
            Assert.AreEqual(backupSat, await Communication.GetStateSaturation(IP, PORT, AUTH_TOKEN));
        }
        [TestMethod]
        public async Task TestGetSetHue()
        {
            await Task.Delay(500);
            var backupHue = await Communication.GetStateHue(IP, PORT, AUTH_TOKEN);
            Assert.IsTrue(await Communication.SetStateHue(IP, PORT, AUTH_TOKEN, 0));
            Assert.AreEqual(0, await Communication.GetStateHue(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateHueIncrement(IP, PORT, AUTH_TOKEN, 180));
            Assert.AreEqual(180, await Communication.GetStateHue(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateHue(IP, PORT, AUTH_TOKEN, backupHue));
            Assert.AreEqual(backupHue, await Communication.GetStateHue(IP, PORT, AUTH_TOKEN));
        }
        [TestMethod]
        public async Task TestGetSetBrightness()
        {
            await Task.Delay(500);
            var backupBrightness = await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN);
            Assert.IsTrue(await Communication.SetStateBrightness(IP, PORT, AUTH_TOKEN, 0));
            Assert.AreEqual(0, await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateBrightnessIncrement(IP, PORT, AUTH_TOKEN, 100));
            Assert.AreEqual(100, await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN));
            Assert.IsTrue(await Communication.SetStateBrightness(IP, PORT, AUTH_TOKEN, 0,1));
            await Task.Delay(1100);
            Assert.AreEqual(0, await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN));
            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetStateBrightness(IP, PORT, AUTH_TOKEN, backupBrightness));
            Assert.AreEqual(backupBrightness, await Communication.GetStateBrightness(IP, PORT, AUTH_TOKEN));
        }

        [TestMethod]
        public async Task TestGetSetOnOff()
        {
            StateEventArgs args = null;
            Communication.StaticOnStateEvent += (o, e) => { args = e; };
            await Communication.RegisterEventListener(IP, PORT, AUTH_TOKEN);
            await Task.Delay(5000);
            Assert.IsTrue(await Communication.SetStateOnOff(IP, PORT, AUTH_TOKEN, false));
            Assert.AreEqual(false, await Communication.GetStateOnOff(IP, PORT, AUTH_TOKEN));
            while (args?.StateEvents?.Events?.FirstOrDefault(e => e.Attribute == EAttribute.On)==null)
                await Task.Delay(1);
            Assert.AreEqual(IP, args.IP);
            Assert.AreEqual(EAttribute.On, args.StateEvents.Events.First(e => e.Attribute == EAttribute.On).Attribute);
            Assert.AreEqual(false, args.StateEvents.Events.First(e => e.Attribute == EAttribute.On).Value);
            args = null;

            await Task.Delay(5000);
            Assert.IsTrue(await Communication.SetStateOnOff(IP, PORT, AUTH_TOKEN, true));
            Assert.AreEqual(true, await Communication.GetStateOnOff(IP, PORT, AUTH_TOKEN));
            while (args?.StateEvents?.Events?.FirstOrDefault(e => e.Attribute == EAttribute.On) == null)
                await Task.Delay(1);
            Assert.AreEqual(IP, args.IP);
            Assert.AreEqual(EAttribute.On, args.StateEvents.Events.First(e => e.Attribute == EAttribute.On).Attribute);
            Assert.AreEqual(true, args.StateEvents.Events.First(e => e.Attribute == EAttribute.On).Value);
            args = null;
            await Task.Delay(5000);
        }
        [TestMethod]
        public async Task TestGetSetGlobalOrientation()
        {
            LayoutEventArgs args = null;
            await Task.Delay(500);
            Communication.StaticOnLayoutEvent += (o, e) => { args = e; };
            await Communication.RegisterEventListener(IP, PORT, AUTH_TOKEN);
            var backupGlobalOrientation = await Communication.GetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN);

            Assert.IsTrue(await Communication.SetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN, 120));
            Assert.AreEqual(120, await Communication.GetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN));
            while(args==null)
                await Task.Delay(1);
            Assert.AreEqual(IP, args.IP);
            Assert.AreEqual(120, args.LayoutEvent.GlobalOrientation);
            args = null;

            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN, 270));
            Assert.AreEqual(270, await Communication.GetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN));
            while (args == null)
                await Task.Delay(1);
            Assert.AreEqual(IP, args.IP);
            Assert.AreEqual(270, args.LayoutEvent.GlobalOrientation);
            args = null;

            await Task.Delay(500);
            Assert.IsTrue(await Communication.SetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN, 0));
            Assert.AreEqual(0, await Communication.GetPanelLayoutGlobalOrientation(IP, PORT, AUTH_TOKEN));
            while (args == null)
                await Task.Delay(1);
            Assert.AreEqual(IP, args.IP);
            Assert.AreEqual(0, args.LayoutEvent.GlobalOrientation);
            args = null;
        }

        [TestMethod]
        public async Task TestIdentify()
        {
            await Task.Delay(500);
            Assert.IsTrue(await Communication.Identify(IP, PORT, AUTH_TOKEN));
            await Task.Delay(5000);
        }
    }
}
