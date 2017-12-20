using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Laba_4
{

    

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static System.Action<DataObject> EjectDevice;

        UsbDetector usbDetector;

        public static System.Action OnUSBDeviceChanded;


        public const int WmDevicechange = 0x0219; // device change event
        public const int DbtDeviceremovecomplete = 0x8004;
        public const int DbtDevicearrival = 0x8000;

        public MainWindow()
        {
            InitializeComponent();

            usbDetector = new UsbDetector();
            usbDetector.StateChanged += new Laba_4.UsbStateChangedEventHandler(usbDetector_StateChanged);

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            DataContext = new ModelView();
            
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper interop = new WindowInteropHelper(this);
            HwndSource hwndSource = HwndSource.FromHwnd(interop.Handle);
            HwndSourceHook hool = new HwndSourceHook(usbDetector.HwndHandler);
            hwndSource.AddHook(hool); ;
            usbDetector.RegisterDeviceNotification(interop.Handle);
        }

        void usbDetector_StateChanged(bool arrival)
        {
            if (OnUSBDeviceChanded != null)
                OnUSBDeviceChanded();
        }
        
        private void Device_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;


            var contextMenu = (ContextMenu)menuItem.Parent;


            var item = (DataGrid)contextMenu.PlacementTarget;
            
            var toDeleteFromBindedList = (DataObject)item.SelectedCells[0].Item;

            if (toDeleteFromBindedList.DeviceType != "MTP")
            {
                if (EjectDevice != null)
                {
                    EjectDevice(toDeleteFromBindedList);
                }
            }
            else
            {
                MessageBox.Show("Cannot eject MTP", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
