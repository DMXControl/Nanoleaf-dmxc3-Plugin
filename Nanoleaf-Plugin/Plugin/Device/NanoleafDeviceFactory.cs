using LumosLIB.Kernel.Devices;
using Nanoleaf_Plugin.Plugin.Device;
using NanoleafAPI;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.Devices.Factory;
using org.dmxc.lumos.Kernel.Exceptions;
using org.dmxc.lumos.Kernel.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nanoleaf_Plugin
{
    sealed class NanoleafDeviceFactory : AbstractDeviceFactory
    {
        static string[] typeNames = { NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME, NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME };
        public override ReadOnlyCollection<DeviceMetadata> AvailableDevices
        {
            get
            {
                var list = new List<DeviceMetadata>();
                foreach (var modell in Enum.GetValues(typeof(EDeviceType)))
                {
                    if (!modell.Equals(EDeviceType.UNKNOWN))
                        list.Add(new DeviceMetadata("Nanoleaf", modell.ToString(), "DMXControl-Projects e.V.", NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME, ""));
                }
                list.Add(new DeviceMetadata("Nanoleaf", "Controller", "DMXControl-Projects e.V.", NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME, ""));
                return list.AsReadOnly();
            }
        }
        public NanoleafDeviceFactory() : base(typeNames)
        {

        }

        public override IDevice createNewInstance(DeviceMetadata m, IDevice parent)
        {
            if (m.Type == NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME)
            {
                NanoleafDevice d = new NanoleafDevice(Guid.NewGuid().ToString());
                d.Name = "new Nanoleaf Panel";
                d.DeviceType = (EDeviceType)Enum.Parse(typeof(EDeviceType), m.Model);

                //Find the next ID if possible
                var lamps = org.dmxc.lumos.Kernel.Project.DeviceManager.getInstance()
                    .Devices.OfType<NanoleafDevice>().Where(c => c.DeviceType.Equals(d.DeviceType)).Select(c => c.PanelID);
                var firstid = NanoleafPlugin.getAllPanels(d.DeviceType).Select(p => p.ID)
                    .Except(lamps)
                    .FirstOrDefault();

                if (firstid != 0)
                {
                    d.PanelID = firstid;
                    return d;
                }

                return d;
            }
            if (m.Type == NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME)
            {
                NanoleafControllerDevice d = new NanoleafControllerDevice(Guid.NewGuid().ToString());
                d.Name = "new Nanoleaf Controller";
                //d.DeviceType = (EDeviceType)Enum.Parse(typeof(EDeviceType), m.Model);

                //Find the next ID if possible
                var lamps = org.dmxc.lumos.Kernel.Project.DeviceManager.getInstance()
                    .Devices.OfType<NanoleafControllerDevice>().Where(c => c.DeviceType.Equals(d.DeviceType)).Select(c => c.SerialNumber);
                var firstid = NanoleafPlugin.getControllers().Select(c => c.SerialNumber)
                    .Except(lamps)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(firstid))
                {
                    d.SerialNumber = firstid;
                    return d;
                }

                return d;
            }
            throw new NotSupportedException("Unable to create a Device for Type: " + m.Type);
        }

        public override IDevice createSavedInstance(ManagedTreeItem item, LumosIOContext context, IDevice parent)
        {
            //If this is the right Factory
            if (!item.Name.Equals(NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME) &&
                !item.Name.Equals(NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME))
                throw new NotSupportedException("Unable to create a Device for Type: " + item.Name);

            string id = item.getValue<string>("ID");
            if (String.IsNullOrEmpty(id))
                throw ItemLoadException.UnableToLoad("ID", "NanoleafDeviceOrController", LumosProtobuf.Resource.EErrorType.Warning);

            if (item.Name.Equals(NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME))
            {
                IDevice d = new NanoleafDevice(id);

                //Delegate the loading of the Values to the Device Class.
                d.loadFromManagedTree(item, context);

                return d;
            }
            else if (item.Name.Equals(NanoleafControllerDevice.NANOLEAF_CONTROLLER_TYPE_NAME))
            {
                IDevice d = new NanoleafControllerDevice(id);

                //Delegate the loading of the Values to the Device Class.
                d.loadFromManagedTree(item, context);

                return d;
            }

            throw new Exception("Could not load Nanoleaf Device or Controller.");
        }
    }
}
