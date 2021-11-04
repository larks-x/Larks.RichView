using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// RichView行容器
    /// </summary>
    public class LineContainer : ElementKey, IDisposable, ICloneable
    {
        private ViewInfo _viewInfo;
        private Graphics graphics;
        private Bitmap lineBitmap;
        /// <summary>
        /// 行内的元素
        /// </summary>
        public List<ViewItem> Contents { get; set; } = new List<ViewItem>();
        /// <summary>
        /// 绘制区域
        /// </summary>
        public RectangleF Rectangle { get; set; } = new RectangleF();
        /// <summary>
        /// 段落样式
        /// </summary>
        public ParagraphAlignment Paragraph = ParagraphAlignment.None;
        /// <summary>
        /// 行内开始Item的索引
        /// </summary>
        public int StartIndex { get; }
        /// <summary>
        /// 行内结束Item的索引
        /// </summary>
        public int EndIndex { get; }
        /// <summary>
        /// 行内字符数
        /// </summary>
        public int ContentCount => Contents.Count;
        /// <summary>
        /// 行容器
        /// </summary>
        /// <param name="viewInfo"></param>
        public LineContainer(ViewInfo viewInfo)
        {
            _viewInfo = viewInfo;
            var layout = _viewInfo.Layout;
            lineBitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
            graphics = Graphics.FromImage(lineBitmap);
            Measure();
            viewInfo.ChangePageSize += () =>
            {
                lineBitmap?.Dispose();
                graphics?.Dispose();
                var layout = _viewInfo.Layout;
                lineBitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
                graphics = Graphics.FromImage(lineBitmap);
                Measure();
            };
        }

        /// <summary>
        /// 确认是否有上一行，并返回上一行信息
        /// </summary>
        /// <returns>Item1:是否存在上一行;Item2:上一行的引用对象</returns>
        internal (bool,LineContainer?) PreviousLine()
        {
            return (true, null);
        }

        /// <summary>
        /// 确认是否有下一行，并返回下一行信息
        /// </summary>
        /// <returns>Item1:是否存在下一行;Item2:下一行的引用对象</returns>
        internal (bool, LineContainer?) NextLine()
        {
            return (true, null);
        }

        /// <summary>
        /// 确定位置,调用此方法后Context不需要重绘
        /// </summary>
        public void DetermineLocation()
        { 
            
        }

        /// <summary>
        /// 测量绘制区域(计算行高和宽度),测量后必须重绘内容
        /// </summary>
        public void Measure()
        { 
            
        }

       
        /// <summary>
        /// 绘制行内容
        /// </summary>
        public async void Draw()
        {
#if NET35 ||NET40
            await TaskNet35.Run(() =>
            {
                foreach (ViewItem item in Contents)
                    item.Draw(graphics, _viewInfo);
            }); 
#else
            await Task.Run(() =>
            {
                foreach (ViewItem item in Contents)
                    item.Draw(graphics, _viewInfo);
            });
#endif
          
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new LineContainer(_viewInfo)
            {
                Contents = Contents.Clone(),
                Rectangle = this.Rectangle,
                Paragraph = this.Paragraph,
            };
        }
    }
}
