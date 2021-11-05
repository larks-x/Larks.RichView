using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ContentElements
{
    /// <summary>
    /// RichView行容器
    /// </summary>
    public class LineContainer : ElementKey, IDisposable, ICloneable
    {
        private ViewInfo _viewInfo;
        private Graphics graphics;
        private Bitmap bitmap;

        private Graphics buffGraphics;
        private Bitmap buffBitmap;

        private object syncLock = new object();

        /// <summary>
        /// 行内的元素
        /// </summary>
        public ContainerList<ContentItem> Contents { get; set; } = new ContainerList<ContentItem>();
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
            bitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
            graphics = Graphics.FromImage(bitmap);
            buffBitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
            buffGraphics = Graphics.FromImage(buffBitmap);

            DetermineLocation();
            Contents.ItemAdd += (item) =>
            {
                item.Measure(buffGraphics, _viewInfo.StyleInfos, _viewInfo.Layout);
                Measure();
            };
            Contents.ItemAddRange += (items) =>
            {
                items.ToList().ForEach((item) =>
                {
                    item.Measure(buffGraphics, _viewInfo.StyleInfos, _viewInfo.Layout);
                });
                Measure();
            };
            _viewInfo.ChangePageSize += () =>
            {
                bitmap?.Dispose();
                graphics?.Dispose();
                buffBitmap?.Dispose();
                buffGraphics?.Dispose();

                var layout = _viewInfo.Layout;
                bitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
                graphics = Graphics.FromImage(bitmap);
                buffBitmap = new Bitmap((int)layout.PageSize.Width - (layout.Padding.Left + layout.Padding.Right), viewInfo.StyleInfos[0].StyleFont.Height);
                buffGraphics = Graphics.FromImage(buffBitmap);
                Measure();
            };
        }

        /// <summary>
        /// 确认是否有上一行，并返回上一行信息
        /// </summary>
        /// <returns>Item1:是否存在上一行;Item2:上一行的引用对象</returns>
        internal (bool,LineContainer?) PreviousLine()
        {
            var curIndex = _viewInfo.Lines.IndexOf(this);
            if (curIndex == 0)
                return (false, null);
            else
                return (true, _viewInfo.Lines[curIndex - 1]);
        }

        /// <summary>
        /// 确认是否有下一行，并返回下一行信息
        /// </summary>
        /// <returns>Item1:是否存在下一行;Item2:下一行的引用对象</returns>
        internal (bool, LineContainer?) NextLine()
        {
            var curIndex = _viewInfo.Lines.IndexOf(this);
            if (curIndex + 1 >= _viewInfo.Lines.Count - 1)
                return (false, null);
            else
                return (true, _viewInfo.Lines[curIndex + 1]);
        }

        /// <summary>
        /// 确定位置,调用此方法后Context不需要重绘
        /// </summary>
        /// <param name="reMeasure">是否需要重新测量高度和宽度</param>
        public void DetermineLocation(bool reMeasure = false)
        {
            var t = PreviousLine();
            PointF p;
            if (!t.Item1)
            {
                p = new PointF(_viewInfo.Layout.Padding.Left, _viewInfo.Layout.Padding.Top); 
            }
            else
            {
                if (t.Item2 != null)
                    p = new PointF(_viewInfo.Layout.Padding.Left, t.Item2.Rectangle.Bottom + _viewInfo.Layout.RowSpacing);
                else
                    throw new Exception("The method [PreviousLine] returns Item1 as true, but Item2 is null");
            }
            var s = new SizeF(_viewInfo.Layout.ClientSize.Width, Rectangle.Height);
            Rectangle = new RectangleF(p, s);
            if (reMeasure)
                Measure();
            
        }

        /// <summary>
        /// 测量绘制区域(计算行高和宽度),测量后必须重绘内容
        /// </summary>
        /// <param name="calculationContentCoordinates">重新计算内容的坐标</param>
        public void Measure(bool calculationContentCoordinates = false)
        {
            List<ContentItem> moveNextLineItem = new List<ContentItem>();
            if (calculationContentCoordinates)
            {
                var tmp =CalculationContent();
                if (tmp.Item1 && tmp.Item2 != null)
                    moveNextLineItem = tmp.Item2;
            }
            //重新测量高度后要传递到下一行
            var nextLine = NextLine();
            if (nextLine.Item1)
            {
                if (nextLine.Item2 != null)
                {
                    if (moveNextLineItem.Count == 0)
                        nextLine.Item2.DetermineLocation(false);
                    else
                    {
                        nextLine.Item2.Contents.InsertRange(0, moveNextLineItem);
                        nextLine.Item2.DetermineLocation(true);
                    }
                }
                else
                    throw new Exception("The method [NextLine] returns Item1 as true, but Item2 is null");
            }
            DrawContent();
        }

        /// <summary>
        /// 计算行内Item的坐标，以及是否有元素要移动到下一行
        /// </summary>
        /// <returns>Item1:是否有数据要移动到下一行中;Item2:要移动到下一行的数据</returns>
        private (bool, List<ContentItem>?) CalculationContent()
        {
            var sumWidth = Contents.Sum(o => o.DrawSize.Width);
            if (sumWidth <= Rectangle.Width)
                return (false, null);
            List<ContentItem> addItems = new List<ContentItem>();
            var startIndex = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                if (i == 0)
                    Contents[i].Location = new PointF(0, 0);
                else
                    Contents[i].Location = new PointF(Contents[i - 1].Location.X, 0);
                if (Contents[i].DrawRectangle.Right > Rectangle.Right)
                {
                    startIndex = Contents.IndexOf(Contents[i]);
                    for (int j = startIndex; j < Contents.Count; j++)
                        addItems.Add(Contents[j]);

                    break;
                }
            }
            Contents.PopRange(startIndex);
            
            return (true, addItems);
        }

        /// <summary>
        /// 绘制行内容
        /// </summary>
        private async void DrawContent()
        {
#if NET35 ||NET40
            await TaskNet35.Run(() =>
            {
                foreach (ContentItem item in Contents)
                    item.Draw(buffGraphics, _viewInfo.StyleInfos);
            });
#else
            await Task.Run(() =>
            {
                foreach (ContentItem item in Contents)
                    item.Draw(buffGraphics,_viewInfo.StyleInfos);
            });
#endif
            lock (syncLock)
            {
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(buffBitmap,0,0);
            }
        }

        public void Draw(Graphics viewGraphics)
        {
            lock (syncLock)
            {
                viewGraphics.DrawImage(bitmap, Rectangle);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            try
            {
                bitmap?.Dispose();
                graphics?.Dispose();
                buffBitmap?.Dispose();
                buffGraphics?.Dispose();
            }
            catch(Exception ex) {
                throw ex;
            }
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
