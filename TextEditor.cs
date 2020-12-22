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
    public partial class TextEditor : Form
    {
        public TextEditor()
        {
            InitializeComponent();
            TEtext.Text = FCMF.textInFile;
        }

        private void TEtext_TextChanged(object sender, EventArgs e)
        {

        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FCMF.pathForSave, false, Encoding.Default))
                {
                    sw.WriteLine(TEtext.Text);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Что ты от меня хочешь? Я просто кнопка, я ничего не знаю.\nСам разбирайся.");
        }
    }
}
