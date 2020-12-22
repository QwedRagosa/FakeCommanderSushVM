using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FakeCommanderSushVM
{
    public partial class DiskInfo : Form
    {
        public DiskInfo()
        {
            InitializeComponent();
            DriveInfo[] di = DriveInfo.GetDrives();
            foreach (DriveInfo d in di)
                listBox1.Items.Add(d.Name);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "";
            DriveInfo di = new DriveInfo(listBox1.SelectedItem.ToString());
            try
            {
                label1.Text = "Имя: " + di.Name + "\n"
                + "Свободное пространство: " + di.AvailableFreeSpace / 1024 / 1024 / 1024 + " Gb\n"
                + "Общий размер: " + di.TotalSize / 1024 / 1024 / 1024 + " Gb\n"
                + "Формат устройства: " + di.DriveFormat + "\n"
                + "Тип устройства: " + di.DriveType;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }
        }
    }
}
