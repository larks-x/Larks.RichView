using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// 内容元素
    /// </summary>
    public class ContentItem : ElementKey, IContentItem
    {
        /// <summary>
        /// RichViewInfo引用
        /// </summary>
        public RichViewInformation RichViewInfo { get; private set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int No => RichViewInfo == null ? -1 : RichViewInfo.ContentItems.IndexOf(this);
        /// <summary>
        /// 类型
        /// </summary>
        public ItemType ItemType { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
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
        public bool IsControlKey { get; internal set; }
        /// <summary>
        /// 控制键
        /// </summary>
        public ControlKey Key { get; internal set; }
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
        public SizeF DrawSize { get; set; }
        /// <summary>
        /// 绘制区域
        /// </summary>
        public RectangleF DrawRectangle => new RectangleF(Location,DrawSize);

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public RectangleF Measure()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 鼠标是否在Item上
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MouseInItem PointInItem(float x, float y)
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
        public void Dispose()
        {
           
        }

        
    }
}
