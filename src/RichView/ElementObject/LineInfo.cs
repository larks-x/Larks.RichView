namespace RichView.ElementObject
{
    /// <summary>
    /// 每行的信息
    /// </summary>
    public class LineInfo: ElementKey,ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        public LineInfo()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="n"></param>
        public LineInfo(float l, float t, float w ,float h, int n)
        {
            Rectangle = new RectangleF(l,t,w,h);
            Number = n;
        }
        /// <summary>
        /// 行的区域
        /// </summary>
        public RectangleF Rectangle { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 左边距离容器的距离
        /// </summary>
        public float Left
        {
            get => Rectangle.Left;
            set {
                if (Rectangle.Left == value)
                    return;
                Rectangle = new RectangleF(value, Rectangle.Top, Rectangle.Width, Rectangle.Height);
            }
        }
        /// <summary>
        /// 顶部坐标
        /// </summary>
        public float Top
        {
            get => Rectangle.Top;
            set
            {
                if (Rectangle.Top == value)
                    return;
                Rectangle = new RectangleF(Rectangle.Left, value, Rectangle.Width, Rectangle.Height);
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonIgnore]
        public float Height
        { 
            get => Rectangle.Height;
            set
            {
                if (Rectangle.Height == value)
                    return;
                Rectangle = new RectangleF(Rectangle.Left, Rectangle.Top, Rectangle.Width, value);
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        [JsonIgnore]
        public float Width
        {
            get => Rectangle.Width;
            set
            {
                if (Rectangle.Width == value)
                    return;
                Rectangle = new RectangleF(Rectangle.Left, Rectangle.Top, value, Rectangle.Height);
            }
        }
        /// <summary>
        /// 底部坐标
        /// </summary>
        public float Bottom => Rectangle.Bottom;

        /// <summary>
        /// 右边界坐标
        /// </summary>
        public float Right => Rectangle.Right;

        /// <summary>
        /// 指定坐标是否在行内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool PointInLine(float x,float y)
        {
            return Rectangle.Contains(x,y);
        }

        /// <summary>
        /// 测量宽度
        /// </summary>
        /// <param name="viewInfo">页面信息</param>
        public void MeasureWidth(ViewInfo viewInfo)
        {
            Width = viewInfo.Layout.PageSize.Width - (viewInfo.Layout.Padding.Left + viewInfo.Layout.Padding.Right);
        }

        /// <summary>
        /// 测量Top值
        /// </summary>
        /// <param name="viewInfo">页面信息</param>
        public void MeasureTop(ViewInfo viewInfo)
        {
            var previousLine = viewInfo.LineInfos.IndexOf(this) - 1;
            if (previousLine < 0)
                previousLine = 0;
            Top = viewInfo.LineInfos[previousLine].Bottom + viewInfo.Layout.RowSpacing;
          
        }

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public LineInfo Copy()
        {
            return new LineInfo(Left,Top,Width,Height,Number);
           
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return Copy();
        }
    }
}
