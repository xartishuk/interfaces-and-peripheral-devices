using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using SimpleWifi;

namespace Laba_7
{
    class ModelView : INotifyPropertyChanged
    {
        public string H1Title => Properties.Resources.H1Title;
        public string WindowTitle => Properties.Resources.WindowTitle;
        public string SendButtonText => Properties.Resources.ConnectButton;
        public string PasswordPlaceholder => Properties.Resources.PasswordPlaceholder;

        private readonly WifiFinder _searcher = new WifiFinder();
        private ObservableCollection<WiFiPoint> networks;

        WiFiPoint _selectedWiFiItem = null;
        public WiFiPoint SelectedWiFiItem
        {
            get
            {
                return _selectedWiFiItem;
            }
            set
            {
                _selectedWiFiItem = value;
                OnPropertyChanged(nameof(SelectedWiFiItem));
            }
        }

        string password;
        public string Password
        {
            get
            {
                if (password == null)
                    return "";
                return password;
            }
            set
            {
                password = value;
            }
        }
        
        private Timer timer;

        private ObservableCollection<WiFiPoint> wifiNetworks;
        public ObservableCollection<WiFiPoint> WifiNetworks
        {
            get
            {
                if (wifiNetworks == null)
                    wifiNetworks = new ObservableCollection<WiFiPoint>();
                return wifiNetworks;
            }

            set
            {
                wifiNetworks = value;
                OnPropertyChanged(nameof(WifiNetworks));
            }
        }
  
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
        private void UpdateWiFiList()
        {
            networks = _searcher.GetWiFiList();
            WifiNetworks = networks;
        }

        private void OnLanChangeList(Object stateInfo)
        {
            UpdateWiFiList();
        }


        private string Dot11BSSTostring(Wlan.WlanBssEntry entry)
        {
            StringBuilder bssIdBuilder = new StringBuilder();
            foreach (byte bssByte in entry.dot11Bssid)
            {
                bssIdBuilder.Append(bssByte.ToString("X"));
                bssIdBuilder.Append("-");
            }
            bssIdBuilder.Remove(bssIdBuilder.Length - 1, 1);
            return bssIdBuilder.ToString();
        }



        public ModelView()
        {
            TimeSpan dueTime = new TimeSpan(0, 0, 0);
            TimeSpan period = new TimeSpan(0, 0, 5);

            TimerCallback timeCB = new TimerCallback(OnLanChangeList);
            timer = new Timer(timeCB, null, dueTime, period);

            
            MainWindow.OnConnectClick += MainWindow_OnConnectClick;



        }


        private string ConvertMac(byte[] macAddr)
        {
            var macAddrLen = (uint)macAddr.Length;
            var str = new string[(int)macAddrLen];

            string mac = "";
            for (int i = 0; i < macAddrLen; i++)
            {
                mac += ((i == 0) ? "" : ":") + macAddr[i].ToString("x2").ToUpper();
            }
            return mac;
        }
        



        private void MainWindow_OnConnectClick()
        {


            if (Password.Length > 0)
            {
                WiFiPoint item = SelectedWiFiItem;

                if(item != null)
                {
                    MessageBoxResult result = MessageBox.Show(String.Format(Properties.Resources.ConnectConfirmText, item.Name), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (item.Connect(Password))
                        {
                            /*ConnectionStatusL.Text = "Connected";
                            PasswordF.Enabled = false;
                            ConnectionB.Enabled = false;
                            NetworkList.SelectedItems[0].Selected = false;*/
                        }
                        else
                        {
                            //ConnectionStatusL.Text = "Error";
                        }
                    }
                    UpdateWiFiList();
                }                
            }
            
        }

    }
}
