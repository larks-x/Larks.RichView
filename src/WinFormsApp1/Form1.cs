using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = false;
            if (fontDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            richView1.ApplyStyle(fontDialog1.Font);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
            if (fontDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            richView1.ApplyStyle(fontDialog1.Font, fontDialog1.Color);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Í¼Æ¬ÎÄ¼þ(*.jpg,*.gif,*.bmp,*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string path = openFileDialog1.FileName;
            Bitmap bitmap = new Bitmap(path);
            richView1.InsertImage(bitmap);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richView1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richView1.Save();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richView1.LoadFile();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button7.Text = richView1.ItemCount().ToString();
        }
    }
}