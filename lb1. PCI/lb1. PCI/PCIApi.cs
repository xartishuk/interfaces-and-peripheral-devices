using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace lb1.PCI
{
    class PCIApi
    {
        private readonly static string GET_PCI_DEVICES_QUERY = "SELECT * FROM Win32_PnPEntity";

        private readonly static string DEVICE_ID = "DeviceID";

        private readonly static string DEVICE_ID_REGEXP_PATTERN = "DEV_.{4}";

        private readonly static string VENDOR_ID_REGEXP_PATTERN = "VEN_.{4}";

        private readonly static string PCI_INFO_FILE = "pci.ids";

        private readonly static string DEVICE_ID_NOT_FOUND_MESSAGE = "Error! DeviceId not found";

        private readonly static string VENDOR_ID_NOT_FOUND_MESSAGE = "Error! VendorId not found";

        public List<PCIDevice> GetPciDevices()
        {
            List<PCIDevice> pciDevices = new List<PCIDevice>();

            ManagementScope connectionScope = new ManagementScope();

            SelectQuery serialQuery = new SelectQuery(GET_PCI_DEVICES_QUERY);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);
            Regex deviceIdRegExp = new Regex(DEVICE_ID_REGEXP_PATTERN);
            Regex vendorIdRegExp = new Regex(VENDOR_ID_REGEXP_PATTERN);

            foreach (var item in searcher.Get())
            {
                string deviceId = item[DEVICE_ID].ToString();
                if (deviceId.Contains("PCI"))
                    try
                    {
                        pciDevices.Add(
                            GetInfo(deviceIdRegExp.Match(deviceId).Value.Substring(4).ToLower(),
                                vendorIdRegExp.Match(deviceId).Value.Substring(4).ToLower())
                        );
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e);
                    }
            }
            return pciDevices;
        }

        private PCIDevice GetInfo(string device, string vendor)
        {
            if (!File.Exists(PCI_INFO_FILE))
            {
                throw new FileNotFoundException("Connot find pci.ids file near .exe file");
            }

            StreamReader pciInfoFile = new StreamReader(PCI_INFO_FILE);

            Regex vendorRegExp = new Regex("^" + vendor + "  ");
            Regex deviceRegExp = new Regex("^\\t" + device + "  ");
            string vendorId = null;
            string deviceId = null;
            while (!pciInfoFile.EndOfStream)
            {
                string vendorBuffer = pciInfoFile.ReadLine();
                if (vendorBuffer != null && vendorRegExp.Match(vendorBuffer).Success)
                {
                    vendorId = "VendorID: " + vendor + " (" + vendorBuffer.Substring(6) + ")";
                    while (!pciInfoFile.EndOfStream)
                    {
                        string deviceBuffer = pciInfoFile.ReadLine();
                        if (deviceBuffer == null || !deviceRegExp.Match(deviceBuffer).Success)
                            continue;

                        deviceId = "DeviceID: " + device + " (" + deviceBuffer.Substring(7) + ")";
                        break;
                    }
                }
            }

            PCIDevice pciDeviceInfo;
            if (deviceId != null && vendorId != null)
            {
                pciDeviceInfo = new PCIDevice(deviceId, vendorId);
            }
            else
            {
                pciDeviceInfo = new PCIDevice(DEVICE_ID_NOT_FOUND_MESSAGE, VENDOR_ID_NOT_FOUND_MESSAGE);
            }

            return pciDeviceInfo;
        }
    }
}
