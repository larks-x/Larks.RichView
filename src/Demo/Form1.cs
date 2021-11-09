using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            richView1.AddLine();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            richView1.Width += 10;
        }
    }
}