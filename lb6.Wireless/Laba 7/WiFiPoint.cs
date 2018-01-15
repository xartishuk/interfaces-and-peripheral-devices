using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_7
{
    public class WiFiPoint
    {
        public string Name { get; set; }
        public string SignalStrength { get; set; }
        public string AuthAlgorithm { get; set; }
        public bool HasProfile { get; set; }
        public bool IsConnected { get; set; }
        public string Mac { get; set; }

       

        public WiFiPoint(string name, string signalStrength, string algorithm, bool hasProfile, bool isSecured, bool isConnected, string mac)
        {
            Name = name;
            SignalStrength = signalStrength;
            AuthAlgorithm = algorithm;
            HasProfile = hasProfile;
            IsConnected = isConnected;
            Mac = mac;
        }



        public bool Connect(string password)
        {
            Wifi wifi = new Wifi();
            AccessPoint accessPoint = wifi.GetAccessPoints().FirstOrDefault(x => x.Name.Equals(Name));
            if (accessPoint != null)
            {
                AuthRequest authRequest = new AuthRequest(accessPoint);
                authRequest.Password = password;
                return accessPoint.Connect(authRequest);
            }
            return false;
        }

    }

    
}
