namespace Larks.RichView.Components
{
    /// <summary>
    /// 内容元素
    /// </summary>
    public class ContentItem : ElementKey, IContentItem
    {
        /// <summary>
        /// RichViewInfo引用
        /// </summary>
        [JsonIgnore]
        public RichViewInformation RichViewInfo { get; private set; }
        /// <summary>
        /// 编号
        /// </summary>
        [JsonIgnore]
        public int No => RichViewInfo == null ? -1 : RichViewInfo.ContentItems.IndexOf(this);
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
        public RectangleF DrawRectangle => new RectangleF(Location,DrawSize);

        public virtual void Draw()
        {
            throw new NotImplementedException();
        }

        public virtual RectangleF Measure()
        {
            throw new NotImplementedException();
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
