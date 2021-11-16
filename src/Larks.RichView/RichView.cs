using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Larks.RichView
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RichView :UserControl
    {
        /// <summary>
        /// 按下一个键
        /// </summary>
        const int WM_KEYDOWN = 0x0100;
        /// <summary>
        /// Tab
        /// </summary>
        const int VK_TAB = 9;
        /// <summary>
        /// Left Arrow
        /// </summary>
        const int VK_LEFT = 37;
        /// <summary>
        /// Up Arrow
        /// </summary>
        const int VK_UP = 38;
        /// <summary>
        /// Right Arrow
        /// </summary>
        const int VK_RIGHT = 39;
        /// <summary>
        /// Down Arrow
        /// </summary>
        const int VK_DOWN = 40;

        RichViewHost host;

        /// <summary>
        /// 
        /// </summary>
        public RichView()
        {
            InitializeComponent();
            ////DoubleBuffer双缓冲，AllPaintingInWmPaint禁止擦除背景
            //this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer
            //| ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw
            //| ControlStyles.SupportsTransparentBackColor
            //, true);
            //this.UpdateStyles();

            host = new RichViewHost(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            host?.WndProc(ref m);
            base.WndProc(ref m);
        }
        /// <summary>
        /// 处理按下方向键和Tab键焦点转移的问题
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.Focused && msg.Msg == WM_KEYDOWN)
            {
                switch ((int)msg.WParam)
                {
                    case VK_TAB:
                    case VK_LEFT:
                    case VK_UP:
                    case VK_RIGHT:
                    case VK_DOWN:
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


    }
}
