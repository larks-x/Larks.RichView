namespace RichView
{
    /// <summary>
    /// RichView的Image元素编辑框,用于调整Image大小
    /// </summary>
    public partial class ImageDesign : UserControl
    {
        private int lastX = 0;
        private int newX = 0;
        /// <summary>
        /// 
        /// </summary>
        public ImageDesign()
        {
            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="img"></param>
        /// <param name="point"></param>
        /// <param name="size"></param>
        public void Bind(Image img, PointF point, SizeF size)
        {

            this.BackgroundImage = img;
            this.Location = point.ToPoint();
            this.Size = size.ToSize();
            this.Visible = true;
        }

        /// <summary>
        /// 解绑
        /// </summary>
        public void UnBind()
        {
            this.Visible = false;
            this.BackgroundImage = null;
        }

        /// <summary>
        /// 当前是否在绑定状态
        /// </summary>
        public bool IsBind => this.Visible;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightBottom_MouseDown(object sender, MouseEventArgs e)
        {
            lastX = e.X;
            newX = lastX;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightBottom_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newX = e.X;
                this.Width += newX - lastX;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightBottom_MouseUp(object sender, MouseEventArgs e)
        {
            int newWidth = 0;
            if (newX != lastX)
                newWidth = newX - lastX;
        }
    }
}
