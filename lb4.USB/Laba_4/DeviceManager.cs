using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MediaDevices;

namespace Laba_4
{


    class DeviceManager
    {
        public List<Device> GetDevices()
        {
            List<Device> usbDevices = new List<Device>();

            List<DriveInfo> diskDrives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Removable).ToList();
            List<MediaDevice> mtpDrives = MediaDevice.GetDevices().ToList();

            foreach (DriveInfo drive in diskDrives)
            {
                usbDevices.Add(new Device(drive));
            }
            
            foreach (MediaDevice device in mtpDrives)
            {

                device.Connect();

                if (device.DeviceType != DeviceType.Generic)
                {
                    usbDevices.Add(new Device(device));
                    Form1.mttp = device;
                    //device.EjectStorage("VID_2717&PID_FF48&MI_02");
                    //device.EjectStorage("USB\\VID_2717&PID_FF48&MI_02");



                    //device.EjectStorage("");
                    //device.EjectStorage("");
                }
            }
            
            return usbDevices;
        }


        
    }
}
