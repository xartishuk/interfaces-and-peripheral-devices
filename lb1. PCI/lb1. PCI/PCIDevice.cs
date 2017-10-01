using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lb1.PCI
{
    class PCIDevice
    {
        private readonly string deviceId;

        private readonly string vendorId;

        public PCIDevice(string deviceId, string vendorId)
        {
            this.deviceId = deviceId;
            this.vendorId = vendorId;
        }

        public string GetDeviceId()
        {
            return deviceId;
        }

        public string GetVendorId()
        {
            return vendorId;
        }
    }
}
