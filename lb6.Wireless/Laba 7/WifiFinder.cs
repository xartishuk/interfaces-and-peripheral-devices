using NativeWifi;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Laba_7
{
    class WifiFinder
    {

        private readonly Wifi _wifi;
        private WlanClient _wlanClient;

        public WifiFinder()
        {
            _wifi = new Wifi();
            _wlanClient = new WlanClient();
        }

        private List<string> GetBssId(AccessPoint accessPoint)
        {
            var wlanInterface = _wlanClient.Interfaces.FirstOrDefault();
            return wlanInterface?.GetNetworkBssList()
                .Where(x => Encoding.ASCII.GetString(x.dot11Ssid.SSID, 0, (int)x.dot11Ssid.SSIDLength).Equals(accessPoint.Name))
                .Select(y => Dot11BSSTostring(y)).ToList();
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

        public ObservableCollection<WiFiPoint> GetWiFiList()
        {
            ObservableCollection<WiFiPoint> a = new ObservableCollection<WiFiPoint>();


            ObservableCollection<WiFiPoint> networks = new ObservableCollection<WiFiPoint>();
            var wlanClient = new WlanClient();
            foreach (WlanClient.WlanInterface wlanInterface in wlanClient.Interfaces)
            {
                List<Wlan.WlanBssEntry> wlanBssEntries = wlanInterface.GetNetworkBssList().ToList();
                List<Wlan.WlanAvailableNetwork> wlanAvalibleEntries = wlanInterface.GetAvailableNetworkList(0).ToList();


                foreach (Wlan.WlanAvailableNetwork wlanEntry in wlanAvalibleEntries)
                {

                    Wlan.WlanBssEntry network = new Wlan.WlanBssEntry();
                    bool finded = false;
                    foreach (Wlan.WlanBssEntry n in wlanBssEntries)
                    {

                        if (System.Text.ASCIIEncoding.ASCII.GetString(n.dot11Ssid.SSID).Equals(System.Text.ASCIIEncoding.ASCII.GetString(wlanEntry.dot11Ssid.SSID)))
                        {
                            network = n;
                            finded = true;
                            wlanBssEntries.Remove(n);
                            break;
                        }
                    }


                    if (!finded)
                        continue;
                    string mac = (finded) ? ConvertMac(network.dot11Bssid) : "";
                    string name = System.Text.ASCIIEncoding.ASCII.GetString(wlanEntry.dot11Ssid.SSID).Trim((char)0);
                    bool hasConnected = (wlanEntry.flags.HasFlag(Wlan.WlanAvailableNetworkFlags.Connected)) ? true : false;
                    bool hasProfile = (wlanEntry.flags.HasFlag(Wlan.WlanAvailableNetworkFlags.HasProfile)) ? true : false;
                    WiFiPoint point = new WiFiPoint(name, wlanEntry.wlanSignalQuality.ToString(), wlanEntry.dot11DefaultAuthAlgorithm.ToString().Trim((char)0), hasProfile, true, hasConnected, mac);
                    /*{
                        SSID = System.Text.ASCIIEncoding.ASCII.GetString(wlanEntry.dot11Ssid.SSID).Trim((char)0),
                        Quality = wlanEntry.wlanSignalQuality.ToString(),
                        AuthAlgorithm = wlanEntry.dot11DefaultAuthAlgorithm.ToString().Trim((char)0),
                        Mac = mac,
                        HasProfile = (wlanEntry.flags.HasFlag(Wlan.WlanAvailableNetworkFlags.HasProfile)) ? true : false,
                    };*/
                    networks.Add(point);
                }

            }



            return networks;
        }
    }
}
