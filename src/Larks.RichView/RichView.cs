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
    public partial class RichView :UserControl
    {
        DrawingManaged DM;
        public RichView()
        {
            InitializeComponent();
            DM=new DrawingManaged(this);
        }
        protected override void WndProc(ref Message m)
        {
            DM?.WndProc(ref m);
            base.WndProc(ref m);
        }
        public void AddLine()
        {
            DM.ContextViewInfo.AddLine();
        }
    }
}
