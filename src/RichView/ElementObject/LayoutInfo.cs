using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 布局信息
    /// </summary>
    public class LayoutInfo:ICloneable
    {
        /// <summary>
        /// 内间距
        /// </summary>
        public Padding Padding { get; set; } = new Padding(3);
        /// <summary>
        /// 行间距(介于0-100之间)
        /// </summary>
        public int RowSpacing { get; set; } = 6;
        /// <summary>
        /// 一个Tab等于几个空格(介于1-10之间)
        /// </summary>
        public int TabToSpace { get; set; } = 2;

        /// <summary>
        /// 页面信息
        /// </summary>
        public SizeF PageSize { get; set; } = new(210, 297);//A4大小

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new LayoutInfo
            {
                Padding = new Padding(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom),
                RowSpacing = RowSpacing,
                TabToSpace = TabToSpace,
                PageSize = PageSize,
            };
        }
    }
}
