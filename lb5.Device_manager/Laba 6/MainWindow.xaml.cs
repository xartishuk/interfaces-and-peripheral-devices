using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Laba_6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static System.Action<Device> OnSelectedDevice;
        public static System.Action<Device> OnMouseDeviceDoubleClick;

        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowModelView();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ListBox)sender).SelectedItem != null)
            {
                OnSelectedDevice?.Invoke((Device)((ListBox)sender).SelectedItem);
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListBox)sender).SelectedItem != null)
            {
                OnMouseDeviceDoubleClick?.Invoke((Device)((ListBox)sender).SelectedItem);
            }
        }
    }
}
