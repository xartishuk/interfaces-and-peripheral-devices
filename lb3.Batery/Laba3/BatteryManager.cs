using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Runtime.InteropServices;

namespace Laba3
{
    class BatteryManager
    {
   
        [DllImport("kernel32")]
        public static extern int GetSystemPowerStatus(ref POWER_STATUS lpSystemPowerStatus);

        private POWER_STATUS powerStatus;

        public POWER_STATUS PowerStatus
        {
            get
            {
                return powerStatus;
            }
            private set
            {
                powerStatus = value;
            }
        }

        
        public BatteryManager()
        {
            PowerStatus = Init();
        }


        public void UpdateInfo()
        {
            PowerStatus = Init();
            GetSystemPowerStatus(ref powerStatus);
        }

        private POWER_STATUS Init()
        {
            POWER_STATUS tempSPS;
            
            tempSPS.ACLineStatus = 1;
            tempSPS.BatteryFlag = 1;
            tempSPS.BatteryLifePercent = 1;
            tempSPS.SystemStatusFlag = 1;
            tempSPS.BatteryLifeTime = 1;
            tempSPS.BatteryFullLifeTime = 1;
            return tempSPS;
        }

        public void ChangeScreenTime(ACDC_STATUS status, float value)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            switch (status)
            {
                case ACDC_STATUS.AC:
                    process.StartInfo.Arguments = "/c powercfg /x -monitor-timeout-ac " + value;
                    break;

                case ACDC_STATUS.DC:
                    process.StartInfo.Arguments = "/c powercfg /x -monitor-timeout-dc " + value;
                    break;
            }
                        
            process.Start();
        }
        

        private string ParseCMD(string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            
            return process.StandardOutput.ReadToEnd();
        }

        public void GetScreenTime(ref float timeWithPower, ref float timeNotPower)
        {
            var powercfg = ParseCMD("/c powercfg /q");

            var information = powercfg.Split(new[] { "GUID" }, StringSplitOptions.None).ToList()
                .Find(i => i.Contains("VIDEOIDLE"))
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            timeWithPower = Convert.ToInt32(information[5].Split(new[] { ' ' }).Last().Split(new[] { 'x' }).Last(), 16) / 60;
            timeNotPower = Convert.ToInt32(information[6].Split(new[] { ' ' }).Last().Split(new[] { 'x' }).Last(), 16) / 60;
        }

    }
}
