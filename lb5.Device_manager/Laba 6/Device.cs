using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Laba_6
{
    public class Device
    {

        #region Fields

        private ManagementObject _device;
        private ManagementObject _driver;

        #endregion

        #region Properties

        public string DeviceID
        {
            get
            {
                return Object2String(_device["DeviceID"]);
            }
        }

        public string Title
        {
            get
            {
                return Object2String(_device["Name"]);
            }
        }

        public string GuID
        {
            get
            {
                return Object2String(_device["ClassGuid"]);
            }
        }

        public string Hardware
        {
            get
            {
                var hardware = _device["HardwareID"] as string[];
                string result = string.Empty;
                foreach (var temp in hardware)
                {
                    if (temp != "(null)")
                    {
                        result += temp + "    ";
                    }
                }
                return result;
            }
        }


        public string Manufacturer
        {
            get
            {
                return Object2String(_device["Manufacturer"]);
            }
        }

        public string DriverDescription
        {
            get
            {
                if (_driver != null)
                {

                    return Object2String(_driver["Description"]);
                }
                return "";
            }
        }
        public string DriverPath
        {
            get
            {
                if (_driver != null)
                {
                    return Object2String(_driver["PathName"]);
                }
                return "";
            }
        }

        public bool IsEnable
        {
            get
            {
                var code = _device["ConfigManagerErrorCode"];
                return !(Convert.ToInt32(code) == 22);
            }
        }

        #endregion


        #region Cns

        public Device(ManagementObject device, ManagementObject driver)
        {
            _device = device;
            _driver = driver;
        }

        public Device()
        {
        }

        #endregion


        #region Public methods

        public bool Disconnect()
        {
            try
            {
                _device.InvokeMethod("Disable", null);
            }
            catch (ManagementException)
            {
                return false;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public bool Connect()
        {
            try
            {
                _device.InvokeMethod("Enable", null);
            }
            catch (ManagementException)
            {
                return false;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        #endregion


        #region Private methods

        private string Object2String(object a)
        {
            return (a != null) ? a.ToString() : " ";
        }

        #endregion
    }
}
