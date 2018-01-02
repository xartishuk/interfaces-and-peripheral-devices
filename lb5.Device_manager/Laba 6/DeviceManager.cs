using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Laba_6
{
    class DeviceManager
    {
        public ObservableCollection<Device> Devices
        {
            get
            {
                var a = GetListDevices();
                var oc = new ObservableCollection<Device>();

                foreach (var item in a)
                {
                    oc.Add(item);
                }

                return oc;
            }
        }

        private List<Device> GetListDevices()
        {
            var serialQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
            var searcher = new ManagementObjectSearcher(new ManagementScope(), serialQuery);

            var devices = searcher.Get();
            var devicesList = new List<Device>();

            foreach (var fDevice in devices)
            {
                if (fDevice["PNPClass"] == null)
                {
                    continue;
                }
                ManagementObject device = (ManagementObject)fDevice;

                var driver = device.GetRelated("Win32_SystemDriver");


                if (driver.Count != 0)
                {
                    devicesList.Add(new Device(device, driver.OfType<ManagementObject>().FirstOrDefault()));
                    continue;
                }
                devicesList.Add(new Device(device, null));
            }

            return devicesList;
        }


    }
}
