using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

using System.Text.RegularExpressions;

namespace Laba3
{
    class ApplicationViewModel : INotifyPropertyChanged
    {  

        #region XAML updater 
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        private Timer timer;
        private float timeWithPower;
        private float timeNotPower;
        private float InitialDCTime = -1;
        private float InitialACTime = -1;
        private readonly BatteryManager batteryManager = new BatteryManager();
        
        public string LineStatus
        {
            get
            {
                if (batteryManager.PowerStatus.ACLineStatus == 1)
                    return "AC";
                return "DC";
            }

        }

        public string BatteryPersent
        {
            get
            {
                if (batteryManager.PowerStatus.BatteryLifePercent > 100)
                {
                    return "-";
                }
                else
                {
                    return batteryManager.PowerStatus.BatteryLifePercent.ToString();
                }
            }

        }
        
        public double BatteryProgress
        {
            get
            {
                if (batteryManager.PowerStatus.BatteryLifePercent > 100)
                {
                    return 100.0;
                }
                return batteryManager.PowerStatus.BatteryLifePercent;
            }
            set
            {
                //OnPropertyChanged("BatteryProgress");
            }
        }
        

        public string BatteryLifeTime
        {
            get {
                if(batteryManager.PowerStatus.BatteryLifeTime != -1)
                {
                    return (batteryManager.PowerStatus.BatteryLifeTime/60).ToString() + " мин";
                }
                return "-";
            }
            
        }

        public float DisplayTimeAC
        {
            get
            {
                return timeWithPower;
            }
            set
            {                
               timeWithPower = value;
                if (timeWithPower!=0)
                {
                    batteryManager.ChangeScreenTime(ACDC_STATUS.AC ,timeWithPower);
                }
                OnPropertyChanged("TimeWithPower");
            }
        }

        public float DisplayTimeDC
        {
            get
            {
                return timeNotPower;
            }

            set
            {
                timeNotPower = value;
                if ( timeNotPower != 0)
                {
                    batteryManager.ChangeScreenTime(ACDC_STATUS.DC, timeNotPower);

                }
                OnPropertyChanged("TimeNotPower");
            }
        }

        public ApplicationViewModel()
        {
            ManageBatteryLevel();

            batteryManager.GetScreenTime(ref timeWithPower, ref timeNotPower);
            InitialDCTime = timeWithPower;
            InitialACTime = timeNotPower;
            MainWindow.OnWindowClosed += OnWindowClosed;
        }

        private void OnWindowClosed()
        {
            batteryManager.ChangeScreenTime(ACDC_STATUS.AC, InitialACTime);
            batteryManager.ChangeScreenTime(ACDC_STATUS.DC, InitialDCTime);
        }

        private void OnBatteryLevelChange(Object stateInfo)
        {
            batteryManager.UpdateInfo();

            OnPropertyChanged("LineStatus");
            OnPropertyChanged("BatteryLifeTime");
            OnPropertyChanged("BatteryPersent");
            OnPropertyChanged("BatteryProgress");
        }

        private void ManageBatteryLevel()
        {
            TimeSpan dueTime = new TimeSpan(0, 0, 0);
            TimeSpan period = new TimeSpan(0, 0, 1);

            TimerCallback timeCB = new TimerCallback(OnBatteryLevelChange);
            timer = new Timer(timeCB, null, dueTime, period);
        }

    }
}

