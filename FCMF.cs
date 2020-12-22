using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FakeCommanderSushVM
{
    public partial class FCMF : Form
    {
        public FCMF()
        {
            InitializeComponent();
            CreateOneList(listView1);
            CreateOneList(listView2);
        }

        public static string textInFile { get; set; }
        public static string NameFile { get; set; }
        public static string pathForSave { get; set; }
        public static string list { get; set; }

        private string PathOneDirectory;

        #region Обработка методов
        private void CreateOneList(ListView liv)
        {
            liv.View = View.Details;
            liv.Columns.Add("Тип");
            liv.Columns[0].Width = 35;
            liv.Columns.Add("Имя файла");
            liv.Columns[1].Width = 80;
            liv.Columns.Add("Расширение");
            liv.Columns[2].Width = 40;
            liv.Columns.Add("Размер");
            liv.Columns[3].Width = 50;
            liv.Columns.Add("Дата создания");
            liv.Columns[4].Width = 115;


            liv.FullRowSelect = true;

        }
        private void EventViewDataOneList(ListView lsv, string folderData, string pattern = "*.*")
        {
            try
            {
                lsv.Items.Clear();
                DirectoryInfo dirinfo = new DirectoryInfo(folderData);
                DirectoryInfo[] directories = dirinfo.GetDirectories();
                FileInfo[] file = dirinfo.GetFiles(pattern);
                lsv.Items.Add(new ListViewItem(new string[] {
                "Go back",
                dirinfo.Parent.Name,
                "...",
                "...",
                "..."
            }));

                foreach (DirectoryInfo directory in directories)
                {

                    lsv.Items.Add(new ListViewItem(new string[]
                        {
                        "Directory",
                        directory.Name,
                        "...",
                        Size(directory),
                        directory.CreationTime.ToString()
                        }
                        ));
                }

                foreach (FileInfo item in file)
                {
                    if (item.Name.Equals("desktop.ini"))
                    {
                        continue;
                    }

                    lsv.Items.Add(new ListViewItem(new string[]
                        {
                        "File",
                        item.Name,
                        item.Extension,
                        fileSize(item.Length),
                        item.CreationTime.ToString()
                        }
                        ));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }

        }

        private string fileSize(long size)
        {
            if (size / 1024 <= 0)
            {
                return size + " Б";
            }
            else if (size / (1024 * 1024) <= 0)
            {
                return size / 1024 + " КБ";
            }
            else if (size / (1024 * 1024 * 1024) <= 0)
            {
                return size / (1024 * 1024) + " МБ";
            }
            else
            {
                return size / (1024 * 1024 * 1024) + " ГБ";
            }
        }

        private string Size(DirectoryInfo data)
        {
            return Directory.GetFiles(
                data.FullName,
                "*",
                SearchOption.AllDirectories)
                .Sum(x => new FileInfo(x).Length / 1024 / 1024).ToString() + " МБ";
        }

        private void ClickHandler(ListView lsv, ref string path, TextBox textBox)
        {
            try
            {
                var SelectItem = lsv.SelectedItems[0];
                string type = SelectItem.SubItems[0].Text;
                DirectoryInfo dirinfo = new DirectoryInfo(path);
                if (type.Equals("Go back"))
                {
                    DirectoryInfo parent = dirinfo.Parent;
                    path = parent.FullName;
                    textBox.Text = path;
                    EventViewDataOneList(lsv, path);
                }
                else if (type.Equals("Directory"))
                {
                    DirectoryInfo[] directories = dirinfo.GetDirectories();
                    foreach (DirectoryInfo dir in directories)
                    {
                        if (dir.Name.Equals(SelectItem.SubItems[1].Text))
                        {
                            path = dir.FullName;
                            textBox.Text = path;
                            EventViewDataOneList(lsv, path);
                        }
                    }
                }
                else if (type.Equals("File"))
                {
                    //Открытие файла в TextBox, при условии что файл с форматом txt
                    var SelectedItem = lsv.SelectedItems[0];
                    string pathForFile = SelectedItem.SubItems[1].Text;
                    FileInfo info = new FileInfo(textBox.Text + "\\" + pathForFile);
                    NameFile = info.Name;
                    pathForSave = textBox.Text + "\\" + pathForFile;
                    using (FileStream fileStream = File.OpenRead(textBox.Text + "\\" + pathForFile))
                    {
                        byte[] array = new byte[fileStream.Length];
                        fileStream.Read(array, 0, array.Length);
                        textInFile = Encoding.Default.GetString(array);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }

        }
        private void deleteFile(ListView listView, TextBox textBox)
        {
            try
            {
                var SelectedItem = listView.SelectedItems[0];
                string path = SelectedItem.SubItems[1].Text;
                FileInfo fileInf = new FileInfo(textBox.Text + "\\" + path);
                if (fileInf.Exists)
                {
                    string message = "Вы действительно хотите удалить этот файл: " + fileInf.Name + "?";
                    string caption = "Удаление файла";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    result = MessageBox.Show(message, caption, buttons);
                    if (result == DialogResult.Yes)
                    {
                        fileInf.Delete();
                        EventViewDataOneList(listView, textBox.Text);
                    }
                }
                else
                    MessageBox.Show("Файл удален.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }
        }
        private void moveAndCopyFile(ListView listView, ListView listView1, TextBox textBox, TextBox newPath, string CopyOrMove)
        {
            try
            {
                var SelectedItem = listView.SelectedItems[0];
                string path = SelectedItem.SubItems[1].Text;
                FileInfo fileInf = new FileInfo(textBox.Text + "\\" + path);
                if (fileInf.Exists && CopyOrMove == "copy")
                {
                    fileInf.CopyTo(newPath.Text + "\\" + path);
                    EventViewDataOneList(listView, textBox.Text);
                    EventViewDataOneList(listView1, newPath.Text);
                }
                else if (fileInf.Exists && CopyOrMove == "move")
                {
                    fileInf.MoveTo(newPath.Text + "\\" + path);
                    EventViewDataOneList(listView, textBox.Text);
                    EventViewDataOneList(listView1, newPath.Text);
                }
                else
                    MessageBox.Show("Операция уже была совершена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }
        }
        #endregion

        private void ButOpen1_Click(object sender, EventArgs e)
        {

        }
        #region Открытие папки
        private void ButOpen1_Click_1(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    PathTB1.Text = fbd.SelectedPath;
                    PathOneDirectory = fbd.SelectedPath;
                    EventViewDataOneList(listView1, fbd.SelectedPath);
                }
            }
        }

        private void ButOpen2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    PathTB2.Text = fbd.SelectedPath;
                    PathOneDirectory = fbd.SelectedPath;
                    EventViewDataOneList(listView2, fbd.SelectedPath);
                }
            }
        }
        #endregion

        #region Двойной тык
        private void ListDataFilesOne_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var SelectedItem = listView1.SelectedItems[0];
                string path = SelectedItem.SubItems[1].Text;
                ClickHandler(listView1, ref PathOneDirectory, PathTB1);
                FileInfo info = new FileInfo(PathTB1.Text + "\\" + path);
                if (info.Extension == ".txt" || info.Extension == ".docx")
                {
                    TextEditor text = new TextEditor();
                    text.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }

        }
        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var SelectedItem = listView2.SelectedItems[0];
                string path = SelectedItem.SubItems[1].Text;
                ClickHandler(listView2, ref PathOneDirectory, PathTB2);
                FileInfo info = new FileInfo(PathTB2.Text + "\\" + path);
                if (info.Extension == ".txt" || info.Extension == ".docx")
                {
                    TextEditor text = new TextEditor();
                    text.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка.");
            }
        }
        #endregion
        #region Поглядим на информацию о дисках
        private void информацияОДискахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiskInfo info = new DiskInfo();
            info.Show();
        }
        #endregion
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FCAbout About = new FCAbout();
            About.Show();
        }

        private void ButExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
