using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        private static extern bool GetCaretPos(out Point lpPoint);
        [DllImport("user32.dll")]
        private static extern bool DestroyCaret();
        public Form1()
        {
            InitializeComponent();
        }
        int x = 1;
        private void button1_Click(object sender, System.EventArgs e)
        {
            x += 5;
            MoveCaretPos(x, 10, 10);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            richView1.Width += 10;
        }

        protected void MoveCaretPos(int x, int y, int height)
        {
            //创建光标
            CreateCaret(richView1.Handle, IntPtr.Zero, 0, height);
            SetCaretPos(x, y);
            //SetCaretBlinkTime(600);
            ShowCaret(richView1.Handle);

        }
    }
}