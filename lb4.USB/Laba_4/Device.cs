using MediaDevices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbEject;

namespace Laba_4
{
    class Device
    {


        #region Properties


        public Object DeviceIdentificator
        {
            get;
            private set;
        }

        public string DeviceFriendlyName
        {
            get;
            private set;
        }


        public string FreeSpace
        {
            get;
            private set;
        }


        public string UsedSpace {
            get;
            private set;
        }


        public string TotalSpace {
            get;
            private set;
        }


        public bool IsMTPDevice
        {
            get;
            private set;
        }

        public string DeviceType
        {
            get
            {
                if(IsMTPDevice)
                {
                    return "MTP";
                }
                return "USB";
            }
        }

        

        #endregion


        #region Constuctors


        public Device(MediaDevice device)
        {
            DeviceIdentificator = device;
            Init(device.FriendlyName, -1, -1, -1, true);
        }


        public Device(DriveInfo device)
        {
            DeviceIdentificator = device;
            long freeSpace = device.TotalFreeSpace;
            long usedSpace = device.TotalSize - device.TotalFreeSpace;
            long totalSize = device.TotalSize;
            Init(device.Name, freeSpace, usedSpace, totalSize, false);
        }


        private void Init(string name, long freeSpace, long usedSpace, long totalSpace, bool isMTP)
        {
            DeviceFriendlyName = name;
            FreeSpace = Byte2MB(freeSpace);
            UsedSpace = Byte2MB(usedSpace);
            TotalSpace = Byte2MB(totalSpace);
            IsMTPDevice = isMTP;
        }



        public bool Eject()
        {
            var tempName = this.DeviceFriendlyName.Remove(2);
            var ejectedDevice = new VolumeDeviceClass().SingleOrDefault(v => v.LogicalDrive == tempName);

            ejectedDevice.Eject(false);
            var tempDevice = new VolumeDeviceClass().SingleOrDefault(v => v.LogicalDrive == tempName);

            if (tempDevice == null)
                return true;

            return false;

        }

        #endregion

        private string Byte2MB(long value)
        {
            if (value < 0)
                return "-";
            double megaBytes = (value / 1024) / 1024;
            return megaBytes.ToString() + " mb";
        }

    }
}
