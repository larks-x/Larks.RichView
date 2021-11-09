namespace Larks.RichView.ContentElements
{
    public class ContentItem : ElementKey, IContentItem
    {
        private List<StyleInfo> _Styles;
        /// <summary>
        /// Item类型
        /// </summary>
        public virtual ItemType ItemType => ItemType.Text;
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 字体风格号
        /// </summary>
        public int StyleNo { get; set; }
        /// <summary>
        /// item所在行
        /// </summary>
        public LineContainer Line { get; set; }
        /// <summary>
        /// 是否为选中状态
        /// </summary>
        public bool IsSelect { get; set; }
        /// <summary>
        /// 是否为控制键
        /// </summary>
        public virtual bool IsControlKey { get; set; }
        /// <summary>
        /// 控制键
        /// </summary>
        public virtual ControlKey Key { get; set; }
        /// <summary>
        /// 绘制坐标
        /// </summary>
        public PointF Location { get; set; }
        /// <summary>
        /// 元素原始大小
        /// </summary>
        public SizeF Size { get; set; }
        /// <summary>
        /// 绘制大小
        /// </summary>
        public virtual SizeF DrawSize { get; set; }

        /// <summary>
        /// 绘制区域
        /// </summary>
        /// <returns></returns>
        public RectangleF DrawRectangle => new RectangleF(new PointF( Line.Location.X + Location.X,Line.Location.Y+Location.Y), DrawSize);

        public ContentItem()
        {

        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 测量元素
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual RectangleF Measure(Graphics graphics, List<StyleInfo> styles, LayoutInfo layout)
        {
            var myMeasureFormat = new System.Drawing.StringFormat(
                    System.Drawing.StringFormat.GenericTypographic);
            myMeasureFormat.FormatFlags = System.Drawing.StringFormatFlags.FitBlackBox
                | System.Drawing.StringFormatFlags.MeasureTrailingSpaces
                | StringFormatFlags.NoClip;
            var style = styles.SingleOrDefault(o => o.StyleNo == StyleNo);
            if (style == null)
                style = styles[0];
            if (!IsControlKey)
                Size = graphics.MeasureString(Text, style.StyleFont, 800, myMeasureFormat);
            else
            {
                Size = SizeF.Empty;
                switch (Key)
                {
                    case ControlKey.Space:
                        var ts = graphics.MeasureString("测", style.StyleFont, 800, StringFormat.GenericTypographic);
                        Size = new SizeF(ts.Width / 2, ts.Height);
                        break;
                    case ControlKey.Tab:
                        var ts1 = graphics.MeasureString("测", style.StyleFont, 800, StringFormat.GenericTypographic);
                        Size = new SizeF((ts1.Width / 2) * layout.TabToSpace, ts1.Height);
                        break;
                }
            }
            return DrawRectangle;
        }



        /// <summary>
        /// 指定点在元素的哪个区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual MouseInItem PointInItem(float x, float y)
        {
            if (!DrawRectangle.Contains(x, y))
                return MouseInItem.No;
            var midPoint = Location.X + (DrawSize.Width / 2);
            if (x >= midPoint)
                return MouseInItem.After;
            else
                return MouseInItem.Befor;
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Draw(Graphics graphics, List<StyleInfo> styles)
        {
            var style = styles.SingleOrDefault(o => o.StyleNo == StyleNo);
            if (style == null)
                style = styles[0];

            if (Key == ControlKey.Enter)
            {

                // graphics.DrawString($"↵", style.StyleFont, style.DrawBrush, DrawEnterPoint);
            }
            else
            {
                graphics.DrawString(Text, style.StyleFont, style.DrawBrush, Location);
            }
        }

        /// <summary>
        /// 判断一个对象是否为此实例
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null)
                return false;

            if (obj.GetType() != this.GetType())
                return false;
            IContentItem other = (IContentItem)obj;
            if (UniqueKey == other.UniqueKey)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取实例的HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return UniqueKey.GetHashCode();
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Dispose()
        {

        }
    }
}
