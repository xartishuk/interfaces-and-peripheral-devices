using MediaDevices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba_4
{
    //Class of form, where we have all important methods for GUI
    public partial class Form1 : Form
    {

        public static MediaDevice mttp;

        #region Fields

        private const int WM_DEVICECHANGE = 0X219;
        private static readonly DeviceManager _manager = new DeviceManager();
        private List<Device> _deviceList;

        private readonly ListView listView1 = new ListView();
        private readonly ContextMenu contextMenu = new ContextMenu();

        #endregion


        #region System override

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                ReloadForm();
            }
        }

        #endregion


        public Form1()
        {
            InitializeComponent();
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnListViewMouseUp);
        }


        #region Event handlers
        private void OnListViewMouseUp(object sender, MouseEventArgs e)
        {
            ListView listView = sender as ListView;

            if (e.Button == MouseButtons.Right)
            {
                ListViewItem item = listView.GetItemAt(e.X, e.Y);
                var device = _deviceList[item.Index];

                if (item != null)
                {
                    if (!device.IsMTPDevice)
                    {
                        bool isEjected = device.Eject();

                        if (isEjected == false)
                        {
                            label1.Text = "Device is BUSY";
                        }
                    }
                    else
                    {
                        int a = 0;
                        //mttp.Dispose();
                        //mttp.Cancel();
                        //mttp.EjectStorage("USB\\VID_2717&PID_FF48&MI_02");
                        //((MediaDevice)device.DeviceIdentificator).EjectStorage("\\\\?\\usb#vid_2717&pid_ff48&mi_00#6&31c93d63&0&0000");
                        label1.Text = "Can not eject MTP device";
                    }
                }
            }
        }

        private void TickTimer(object sender, EventArgs e)
        {
            ReloadForm();
        }

        #endregion

        private void LoadForm(object sender, EventArgs e)
        {

            listView1.Bounds = new Rectangle(new Point(10, 10), new Size(400, 150));
            
            listView1.View = View.Details;
            listView1.LabelEdit = false;
            listView1.AllowColumnReorder = true;
            listView1.CheckBoxes = false;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Sorting = SortOrder.Ascending;
            
            // Width of -2 indicates auto-size.
            listView1.Columns.Add("Device Name", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Device Type", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Total Space", -2, HorizontalAlignment.Center);
            listView1.Columns.Add("Free Space", -2, HorizontalAlignment.Center);
            listView1.Columns.Add("Used Space", -2, HorizontalAlignment.Center);
            
            /*ImageList imageListSmall = new ImageList();
            imageListSmall.Images.Add(Bitmap.FromFile("D:\\invertain\\TestProject\\Assets\\Textures\\bonus_pack.png"));
            imageListSmall.Images.Add(Bitmap.FromFile("D:\\invertain\\TestProject\\Assets\\Textures\\bonus_pack.png"));
            listView1.SmallImageList = imageListSmall;*/
            
            this.Controls.Add(listView1);



            _deviceList = new List<Device>();
            ReloadForm();
            timer.Enabled = true;
        }


        private void ReloadForm()
        {

            listView1.Items.Clear();
            _deviceList = _manager.GetDevices();

            foreach(Device device in _deviceList)
            {
                ListViewItem item1 = new ListViewItem(device.DeviceFriendlyName, 0);

                item1.Checked = false;
                item1.SubItems.Add(device.DeviceType);
                item1.SubItems.Add(device.TotalSpace);
                item1.SubItems.Add(device.FreeSpace);
                item1.SubItems.Add(device.UsedSpace);
                listView1.Items.Add(item1);
            }

            label1.Text = "";
        }


        
       
    }
}
