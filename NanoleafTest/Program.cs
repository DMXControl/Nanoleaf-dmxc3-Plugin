using LumosLIB.Tools;
using Nanoleaf_Plugin.API;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NanoleafTest
{
    class Program
    {
        const string ip = "192.168.1.123";
        const string port = "16021";
        static Controller controler = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter 5 times for Shutdown");
            Communication.DeviceDiscovered += Communication_DeviceDiscovered;
            Communication.StaticOnTouchEvent += Communication_StaticOnTouchEvent;
            Communication.StaticOnLayoutEvent += Communication_StaticOnLayoutEvent;
            Communication.StaticOnGestureEvent += Communication_StaticOnGestureEvent;
            Communication.StaticOnEffectEvent += Communication_StaticOnEffectEvent;
            Communication.StaticOnStateEvent += Communication_StaticOnStateEvent;
            controler = new Controller(ip);
            bool alive = true;
            Thread taskStream = new Thread(() =>
            {
                byte val = 0;
                while (alive)
                {
                    var rgbw = new Panel.RGBW(val, 0, 0, 0);
                    controler.Panels.ForEach(p => p.StreamingColor = rgbw);
                    Task.Delay(1).Wait();
                    val++;
                }
            });
            taskStream.Start();


            Console.ReadLine();
            controler.SelfDestruction();
            Console.WriteLine("User Deleted");
            alive = false;

            Console.ReadLine();
        }

        private static void Communication_StaticOnStateEvent(object sender, StateEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{e.IP}: StateEvent: EventsCount:{e.StateEvents.Events.Count()}");
            foreach (var _event in e.StateEvents.Events)
                Console.WriteLine(_event.ToString());
        }

        private static void Communication_StaticOnEffectEvent(object sender, EffectEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{e.IP}: EffectEvent: EventsCount:{e.EffectEvents.Events.Count()}");
            foreach (var _event in e.EffectEvents.Events)
                Console.WriteLine(_event.ToString());
        }

        private static void Communication_StaticOnGestureEvent(object sender, GestureEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{e.IP}: GestureEvent: EventsCount:{e.GestureEvents.Events.Count()}");
            foreach (var _event in e.GestureEvents.Events)
                Console.WriteLine(_event.ToString());
        }

        private static void Communication_StaticOnLayoutEvent(object sender, LayoutEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{e.IP}: Layout Changed: GlobalOrientation: {e.LayoutEvent.GlobalOrientation} NumberOfPanels: {e.LayoutEvent.Layout.NumberOfPanels} SideLength: {e.LayoutEvent.Layout.SideLength}");
            foreach (var pp in e.LayoutEvent.Layout.PanelPositions)
                Console.WriteLine(pp.ToString());
        }

        private static void Communication_StaticOnTouchEvent(object sender, TouchEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{e.IP}: TouchEvent: TouchedPanels{e.TouchEvent.TouchedPanelsNumber} EventsCount:{e.TouchEvent.TouchPanelEvents.Count}");
            foreach (var _event in e.TouchEvent.TouchPanelEvents)
            {
                if (_event.PanelIdSwipedFrom.HasValue)
                    Console.WriteLine($"PanelID: {_event.PanelId}, {_event.Type}, SwipedID: {_event.PanelIdSwipedFrom} , Strength:{_event.Strength}");
                else
                    Console.WriteLine($"PanelID: {_event.PanelId}, {_event.Type}, Strength:{_event.Strength}");
            }
        }
        private static void Communication_DeviceDiscovered(object sender, DiscoveredEventArgs e)
        {
            Console.WriteLine($"Device Discovered: {e.DiscoveredDevice.ToString()}");
        }
    }
}
