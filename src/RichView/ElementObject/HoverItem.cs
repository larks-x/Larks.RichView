namespace RichView.ElementObject
{
    /// <summary>
    /// 鼠标悬停的Item信息
    /// </summary>
    public class HoverItem
    {
        /// <summary>
        /// 元素编号
        /// </summary>
        public int ItemNo { get; set; }
        /// <summary>
        /// 鼠标悬停所在的Item
        /// </summary>
        public ViewItem InItem { get; set; }
        /// <summary>
        /// 元素的绘制位置
        /// </summary>
        public RectangleF ItemRectangle { get; set; }
        /// <summary>
        /// 鼠标悬停在Item哪个区域的信息
        /// </summary>
        public MouseInItem Area { get; set; }
    }
}
