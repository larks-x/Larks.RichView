namespace Larks.RichView.Interface
{
    public interface IContentItem : ICloneable, IDisposable
    {
        /// <summary>
        /// RichViewInfo引用
        /// </summary>
        RichViewInformation RichViewInfo { get; set; }
        /// <summary>
        /// 元素编号
        /// </summary>
        int No { get; }
        /// <summary>
        /// 行号
        /// </summary>
        int LineNo { get; set; }
        /// <summary>
        /// 唯一键
        /// </summary>
        long UniqueKey { get; }
        /// <summary>
        /// Item类型
        /// </summary>
        ItemType ItemType { get; }
        /// <summary>
        /// 实际文本
        /// </summary>
        string Text { get; }
        /// <summary>
        /// 绘制文本
        /// </summary>
        string DrawText { get; }
        /// <summary>
        /// 文本风格
        /// </summary>
        int StyleNo { get; set; }
        ///// <summary>
        ///// 所在行
        ///// </summary>
        //LineContainer Line { get; set; }
        /// <summary>
        /// 是否为选中状态
        /// </summary>
        bool IsSelect { get; set; }
        /// <summary>
        /// 是否为控制符
        /// </summary>
        bool IsControlKey { get; }
        /// <summary>
        /// 控制符
        /// </summary>
        ControlKey Key { get; }
        /// <summary>
        /// 绘制坐标
        /// </summary>
        PointF Location { get; set; }
        /// <summary>
        /// 元素原始大小
        /// </summary>
        SizeF Size { get; set; }
        /// <summary>
        /// 元素缩放绘制的大小(对于Image类型的元素需要在此实现缩放)
        /// </summary>
        SizeF DrawSize { get; }
        /// <summary>
        /// 绘制区域
        /// </summary>
        /// <returns></returns>
        RectangleF DrawRectangle { get; }
        /// <summary>
        /// 将行内绘制区域转换为View中的区域
        /// </summary>
        RectangleF DrawRectangleToViewRectangle { get; }
        /// <summary>
        /// 设置或获取Bottom
        /// </summary>
        float Bottom { get; set; }
        /// <summary>
        /// 有边界
        /// </summary>
        float Right { get; }
        /// <summary>
        /// 设置在行内的坐标
        /// </summary>
        /// <param name="left">左边界</param>
        /// <param name="bottom">下边界</param>
        void SetLocationInLine(float left, float bottom);
        /// <summary>
        /// 测量元素并返回元素的绘制区域
        /// </summary>
        /// <returns></returns>
        RectangleF Measure();
        /// <summary>
        /// 计算位置
        /// </summary>
        void CalculationLocation();
        /// <summary>
        /// 指定点在元素的哪个区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        MouseInItem PointInItem(float x, float y);

        /// <summary>
        /// 绘制元素
        /// </summary>
        void Draw();
        /// <summary>
        /// 在指定画布绘制元素
        /// </summary>
        /// <param name="graphics">画布</param>
        void Draw(Graphics graphics);
    }
}
