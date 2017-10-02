using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lb1.PCI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PCIApi pciApi = new PCIApi();
            List<PCIDevice> devices = pciApi.GetPciDevices();
            int i = 1;
            foreach (var device in devices)
            {
                DeviceList.Items.Add(i++ + ". " + device.GetDeviceId());
                DeviceList.Items.Add("    " + device.GetVendorId());
                DeviceList.Items.Add("-----------------------------------------------------------");
            }
        }
    }
}
