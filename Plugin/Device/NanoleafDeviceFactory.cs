using LumosLIB.Kernel;
using LumosLIB.Kernel.Devices;
using LumosLIB.Tools;
using org.dmxc.lumos.Kernel.Devices;
using org.dmxc.lumos.Kernel.Devices.Factory;
using org.dmxc.lumos.Kernel.Exceptions;
using org.dmxc.lumos.Kernel.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_Plugin
{
    sealed class NanoleafDeviceFactory : AbstractDeviceFactory
    {
        public override ReadOnlyCollection<DeviceMetadata> AvailableDevices
        {
            get
            {
                return new List<DeviceMetadata>()
                {
                    new DeviceMetadata("Nanoleaf", "Nanoleaf", LumosConstants.PROGRAM_COMPANY, NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME, "")
                }.AsReadOnly();
            }
        }
        public NanoleafDeviceFactory() : base(NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME)
        {

        }

        public override IDevice createNewInstance(DeviceMetadata m, IDevice parent)
        {
            if (m.Type == NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME)
            {
                NanoleafDevice d = new NanoleafDevice(Guid.NewGuid().ToString());
                d.Name = "new Nanoleaf Panel";

                //Find the next ID if possible
                var lamps = org.dmxc.lumos.Kernel.Project.DeviceManager.getInstance()
                    .Devices.OfType<NanoleafDevice>().Select(c => c.PanelID);
                var firstid = NanoleafPlugin.getAllPanels().Select(p=>p.ID)
                    .Except(lamps)
                    .FirstOrDefault();

                if (firstid!=0)
                {
                    d.PanelID = firstid;
                    return d;
                }

                return d;
            }
            throw new NotSupportedException("Unable to create a Device for Type: " + m.Type);
        }

        public override IDevice createSavedInstance(ManagedTreeItem item, LumosIOContext context, IDevice parent)
        {
            //If this is the right Factory
            if (!item.Name.Equals(NanoleafDevice.NANOLEAF_DEVICE_TYPE_NAME))
                throw new NotSupportedException("Unable to create a Device for Type: " + item.Name);

            string id = item.getValue<string>("ID");
            if (String.IsNullOrEmpty(id))
                throw ItemLoadException.UnableToLoad("ID", "ColorBridgeDevice");

            IDevice d = new NanoleafDevice(id);

            //Delegate the loading of the Values to the Device Class.
            d.loadFromManagedTree(item, context);

            return d;
        }
    }
}
