namespace Larks.RichView.Components
{
    /// <summary>
    /// 内容元素
    /// </summary>
    public class ContentItem : ElementKey, IContentItem
    {
        private RichViewInformation _RichViewInfo = null;
        private StringFormat MeasureFormat=StringFormat.GenericTypographic;

        public ContentItem()
        {
            MeasureFormat.FormatFlags = System.Drawing.StringFormatFlags.FitBlackBox
                | System.Drawing.StringFormatFlags.MeasureTrailingSpaces
                | StringFormatFlags.NoClip;
        }

        /// <summary>
        /// RichViewInfo引用
        /// </summary>
        [JsonIgnore]
        public RichViewInformation RichViewInfo {
            get => _RichViewInfo;
            set {
                if (_RichViewInfo != null)
                    return;
                _RichViewInfo = value;
                if (!_RichViewInfo.UseLineModel)
                {
                    _RichViewInfo.OnDraw += (graphics) =>
                    {
                        Draw();
                    };
                }
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        [JsonIgnore]
        public int No
        {
            get {
                if (RichViewInfo == null)
                    return -1;
                if (RichViewInfo == null && LineNo == -1)
                    return -1;
                if (LineNo > -1)
                    return RichViewInfo.ContentLines[LineNo].Items.IndexOf(this);
                else
                    return RichViewInfo.ContentItems.IndexOf(this);
            }
        }

        /// <summary>
        /// 行内编号
        /// </summary>
        [JsonIgnore]
        public int NoInLine
        {
            get {
                if (LineNo == -1)
                    return -1;
                return RichViewInfo.ContentLines[LineNo].Items.IndexOf(this);
            }
        }

        private int _LineNo = -1;
        /// <summary>
        /// 行号
        /// </summary>
        public int LineNo
        {
            get => _LineNo;
            set {
                if (_LineNo == value)
                    return;
                _LineNo = value;
                CalculationLocation();
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public virtual ItemType ItemType { get; set; }
        /// <summary>
        /// 实际文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 绘制文本
        /// </summary>
        [JsonIgnore]
        public virtual string DrawText
        {
            get => Text;
            set {
                if (Text == value)
                    return;
                Text = value;
            }
        }
        /// <summary>
        /// Style编号
        /// </summary>
        public int StyleNo { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelect { get; set; }
        /// <summary>
        /// 是否为控制键
        /// </summary>
        public virtual bool IsControlKey { get; internal set; }
        /// <summary>
        /// 控制键
        /// </summary>
        public virtual ControlKey Key { get; internal set; }
        /// <summary>
        /// 左上角绘制位置
        /// </summary>
        public PointF Location { get; set; }
        /// <summary>
        /// 实际大小
        /// </summary>
        public SizeF Size { get; set; }
        /// <summary>
        /// 绘制大小
        /// </summary>
        public virtual SizeF DrawSize { get; set; }
        /// <summary>
        /// 绘制区域
        /// </summary>
        [JsonIgnore]
        public RectangleF DrawRectangle => new RectangleF(Location, DrawSize);
        /// <summary>
        /// 将行内绘制区域转换为在View中的区域
        /// </summary>
        [JsonIgnore]
        public RectangleF DrawRectangleToViewRectangle
        {
            get {
                if (LineNo == -1)
                    return DrawRectangle;
                var lineLocation = RichViewInfo.ContentLines[LineNo].DrawRectangle.Location;
                return new RectangleF(new PointF(lineLocation.X+ Location.X, lineLocation.Y+Location.Y), DrawSize);
            }
        }
       
        /// <summary>
        /// 设置或获取Bottom
        /// </summary>
        public float Bottom {
            get => DrawRectangle.Bottom;
            set {
                if (DrawRectangle.Bottom == value)
                    return;
                Location = new PointF(Location.X, value - DrawSize.Height);
            }
        }
        /// <summary>
        /// 有边界
        /// </summary>
        public float Right => DrawRectangle.Right;

        /// <summary>
        /// 设置在行内的坐标
        /// </summary>
        /// <param name="left">左边界</param>
        /// <param name="bottom">下边界</param>
        public void SetLocationInLine(float left, float bottom)
        {
            Location = new PointF(left, bottom - DrawSize.Height);
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public virtual void Draw()
        {
            RichViewInfo.BuffGraphics.DrawString(Text, RichViewInfo.Styles[StyleNo].StyleFont, RichViewInfo.Styles[StyleNo].DrawBrush, DrawRectangle, StringFormat.GenericTypographic);
        }

        /// <summary>
        /// 在指定画布内绘制
        /// </summary>
        /// <param name="graphics"></param>
        public virtual void Draw(Graphics graphics)
        {
            graphics.DrawString(Text, RichViewInfo.Styles[StyleNo].StyleFont, RichViewInfo.Styles[StyleNo].DrawBrush, DrawRectangle, MeasureFormat);
            //graphics.DrawString(Text, RichViewInfo.Styles[StyleNo].StyleFont, RichViewInfo.Styles[StyleNo].DrawBrush, DrawRectangle);
            //graphics.DrawString(Text, RichViewInfo.Styles[StyleNo].StyleFont, RichViewInfo.Styles[StyleNo].DrawBrush, Location);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <returns></returns>
        public virtual RectangleF Measure()
        {
            if (IsControlKey)
            {
                //空格只占半个中文的宽度
                Size = RichViewInfo.BuffGraphics.MeasureString("测", RichViewInfo.Styles[0].StyleFont, 800, MeasureFormat);
                Size = new SizeF( Size.Width / 2,Size.Height);
                if (Key == ControlKey.Tab)
                    Size = new SizeF(Size.Width * RichViewInfo.Layout.TabToSpace, Size.Height); 
                if (Key == ControlKey.Enter)
                    Size = new SizeF(1, Size.Height);
            }
            else
            {
                Size = RichViewInfo.BuffGraphics.MeasureString(Text, RichViewInfo.Styles[StyleNo].StyleFont, 800, MeasureFormat);
                //Size=new SizeF(Size.Width+2, Size.Height+2);
            }
            CalculationLocation();
            return DrawRectangle;
        }

        /// <summary>
        /// 计算位置
        /// </summary>
        /// <returns></returns>
        public void CalculationLocation()
        {
            var pItem = Previous();
            var nItem = Next();
            if (!RichViewInfo.UseLineModel)
            {
                if (pItem == null)
                    Location = new PointF(RichViewInfo.Layout.Padding.Left, RichViewInfo.Layout.Padding.Top);
                else
                {
                    if (pItem.DrawRectangle.Right + DrawSize.Width <= RichViewInfo.Layout.PageSize.Width - RichViewInfo.Layout.Padding.Right)
                        Location = new PointF(pItem.DrawRectangle.Right, pItem.Location.Y);
                    else
                        Location = new PointF(RichViewInfo.Layout.Padding.Left, pItem.DrawRectangle.Bottom + RichViewInfo.Layout.RowSpacing);
                }
                if (nItem != null)
                    nItem.CalculationLocation();
            }
            else
            {
                if (pItem == null)
                    SetLocationInLine(0, RichViewInfo.ContentLines[LineNo].Height);
                else
                    SetLocationInLine(pItem.DrawRectangle.Right, RichViewInfo.ContentLines[LineNo].Height);
                if (nItem != null)
                    nItem.CalculationLocation();
            }  
        }

        /// <summary>
        /// 前一个
        /// </summary>
        /// <returns></returns>
        public IContentItem? Previous()
        {
            if (No <= 0)
                return null;
            if(LineNo==-1)
                return RichViewInfo.ContentItems[No - 1];
            else
                return RichViewInfo.ContentLines[LineNo].Items[No - 1];
        }
        /// <summary>
        /// 后一个
        /// </summary>
        /// <returns></returns>
        public IContentItem? Next()
        {

            if (LineNo == -1)
            {
                if (No == RichViewInfo.ContentItems.Count - 1)
                    return null;
                else
                    return RichViewInfo.ContentItems[No + 1];
            }  
            else
            {
                if (No == RichViewInfo.ContentLines[LineNo].Items.Count - 1)
                    return null;
                return RichViewInfo.ContentLines[LineNo].Items[No + 1];
            }
                
        }

        /// <summary>
        /// 鼠标是否在Item上
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual MouseInItem PointInItem(float x, float y)
        {
            var rect = DrawRectangle;
            if (!rect.Contains(x, y))
                return MouseInItem.No;
            var midPoint = rect.Left + (DrawSize.Width / 2);
            if (x >= midPoint)
                return MouseInItem.After;
            else
                return MouseInItem.Befor;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
           
        }

        
    }
}
