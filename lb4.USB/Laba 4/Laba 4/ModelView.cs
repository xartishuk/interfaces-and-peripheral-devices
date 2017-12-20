using MediaDevices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Laba_4
{
    public class DataObject
    {
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string TotalSpace { get; set; }
        public string FreeSpace { get; set; }
        public string UsedSpace { get; set; }
        public Device Identificator { get; set; }
    }




    class ModelView: INotifyPropertyChanged
    {
        bool isUpdatedAfterEject = true;

        private List<DataObject> wifiNetworks = new List<DataObject>();
        public List<DataObject> WifiNetworks
        {
            get
            {
                if (wifiNetworks == null)
                    wifiNetworks = new List<DataObject>();
                return wifiNetworks;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        

        List<DataObject> _devices = new List<DataObject>();
        public List<DataObject> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");

            }
        }


        public List<DataObject> AllDevices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                OnPropertyChanged("AllDevices");

            }
        }

        public ModelView()
        {
            Devices = new List<DataObject>();
            MainWindow.OnUSBDeviceChanded += usbDetector_StateChanged;
            MainWindow.EjectDevice += MainWindow_OnEjectDevice;
            UpdateDevices();
            

        }

        private void MainWindow_OnEjectDevice(DataObject Name)
        {
            isUpdatedAfterEject = true;
            //new Thread(() =>
            //{
                if (Name.Identificator.Eject())
                {
                    MessageBox.Show("Success", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Some problems", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                isUpdatedAfterEject = false;

                UpdateDevices();
            //}).Start();

        }

        void usbDetector_StateChanged()
        {
            UpdateDevices();
        }
        

        public void UpdateDevices()
        {
            
            var asd = GetDevices();

            var c = new List<DataObject>();
            
            foreach (var a in asd)
            {
                if(!a.DeviceFriendlyName.Equals(""))
                c.Add(new DataObject()
                {
                    DeviceName = a.DeviceFriendlyName,
                    DeviceType = a.DeviceType,
                    TotalSpace = a.TotalSpace,
                    FreeSpace = a.FreeSpace,
                    UsedSpace = a.UsedSpace,
                    Identificator = a,
                });


            }

            Devices = c;
            OnPropertyChanged("Devices");

        }


        protected List<Device> GetDevices()
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

                try
                {
                    device.Connect();

                    if (device.DeviceType != DeviceType.Generic)
                    {
                        usbDevices.Add(new Device(device));

                    }
                }
                catch (FileNotFoundException a)
                {
                    
                }
            }

            return usbDevices;
        }
    }
    
}
