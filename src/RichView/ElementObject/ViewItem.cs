using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewItem : ElementKey, IViewItem
    {
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
        /// 行号
        /// </summary>
        public int LineNo { get; set; }
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
        public RectangleF DrawRectangle => new RectangleF(Location, DrawSize);

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
        /// <param name="viewInfo">ViewInfo</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual RectangleF Measure(Graphics graphics, ViewInfo viewInfo)
        {
            var myMeasureFormat = new System.Drawing.StringFormat(
                    System.Drawing.StringFormat.GenericTypographic);
            myMeasureFormat.FormatFlags = System.Drawing.StringFormatFlags.FitBlackBox
                | System.Drawing.StringFormatFlags.MeasureTrailingSpaces
                | StringFormatFlags.NoClip;
            RectangleF previousItemRect = RectangleF.Empty;
            int curLineNo = 0;
            //var lastItemIndex = viewInfo.ContextItems.IndexOf(this) - 1;
            var lastItemIndex = viewInfo.ContextItems.Select((ViewItem, index) => new { index, ViewItem }).FirstOrDefault(o => o.ViewItem.UniqueKey == this.UniqueKey).index -1;
           
            if (lastItemIndex < 0)
                previousItemRect = new RectangleF(viewInfo.Layout.Padding.Left, 0, 0, 0);
            else
            {
                previousItemRect = viewInfo.ContextItems[lastItemIndex].DrawRectangle;
                curLineNo = viewInfo.ContextItems[lastItemIndex].LineNo;
            }
            LineNo = curLineNo;
            var style = viewInfo.StyleInfos.SingleOrDefault(o => o.StyleNo == StyleNo);
            if (style == null)
                style = viewInfo.StyleInfos[0];
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
                        Size = new SizeF((ts1.Width / 2) * viewInfo.Layout.TabToSpace, ts1.Height);
                        break;
                }
            }
            //Size = new SizeF(10, 10);
            Location = new PointF(previousItemRect.Right, viewInfo.LineInfos[LineNo].Top);
            if (DrawRectangle.Right > viewInfo.LineInfos[LineNo].Right || Key == ControlKey.Enter)
            {
                LineNo++;
                TryAddLine(viewInfo,LineNo);
                Location = new PointF(viewInfo.Layout.Padding.Left, viewInfo.LineInfos[LineNo].Top);
            }
            return DrawRectangle;
        }

        /// <summary>
        /// 尝试追加一行
        /// </summary>
        /// <param name="viewInfo"></param>
        /// <param name="lineNo">行号</param>
        public void TryAddLine(ViewInfo viewInfo,int lineNo)
        {
            if (viewInfo.LineInfos.SingleOrDefault(o => o.Number == lineNo) == null)
            {
                viewInfo.LineInfos.Add(new LineInfo() { Number = LineNo, Left = viewInfo.Layout.Padding.Left, Top = 0 });
                viewInfo.LineInfos[LineNo].MeasureWidth(viewInfo);
            }
        }

        /// <summary>
        /// 行高发生变化时触发
        /// </summary>
        /// <param name="lineInfo">行信息</param>
        public virtual void LineHeightChange(LineInfo lineInfo)
        {
            Location = new PointF(Location.X, lineInfo.Bottom - DrawSize.Height);
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
        /// <param name="viewInfo">ViewInfo</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Draw(Graphics graphics,ViewInfo viewInfo)
        {
            var style = viewInfo.StyleInfos.SingleOrDefault(o => o.StyleNo == StyleNo);
            if (style == null)
                style = viewInfo.StyleInfos[0];
            var lineInfo = viewInfo.LineInfos[LineNo];
            if (Key == ControlKey.Enter)
            {
                PointF DrawEnterPoint = PointF.Empty;
                var lastItemIndex = viewInfo.ContextItems.IndexOf(this) - 1;
                if (lastItemIndex < 0)
                    DrawEnterPoint = new PointF(viewInfo.Layout.Padding.Left + 2, viewInfo.Layout.Padding.Top+(lineInfo.Height- style.StyleFont.Height)/2);
                else
                {
                    var lastItem = viewInfo.ContextItems[lastItemIndex];
                    DrawEnterPoint = new PointF(lastItem.DrawRectangle.Right + 2, viewInfo.Layout.Padding.Top + (lineInfo.Height - style.StyleFont.Height) / 2);
                }

                graphics.DrawString($"↵", style.StyleFont, style.DrawBrush, DrawEnterPoint);
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
            IViewItem other = (IViewItem)obj;
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
