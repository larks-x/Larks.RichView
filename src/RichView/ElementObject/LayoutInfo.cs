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
        /// layout发生改变
        /// </summary>
        public Action<LayoutChangeProperty> LayoutChange;

        private Padding _Padding = new Padding(3);
        /// <summary>
        /// 内间距
        /// </summary>
        public Padding Padding
        {
            get => _Padding;
            set {
                if (_Padding == value)
                    return;
                Padding = value;
                LayoutChange?.Invoke(LayoutChangeProperty.Padding);
            }
        }

        private int _RowSpacing = 6;
        /// <summary>
        /// 行间距(介于0-100之间)
        /// </summary>
        public int RowSpacing
        {
            get => _RowSpacing;
            set
            {
                if (_RowSpacing == value)
                    return;
                if (value < 0)
                    _RowSpacing = 0;
                else if (value > 100)
                    _RowSpacing = 100;
                else
                    _RowSpacing = value;
                LayoutChange?.Invoke(LayoutChangeProperty.RowSpacing);
            }
        }

        private int _TabToSpace = 2;
        /// <summary>
        /// 一个Tab等于几个空格(介于1-10之间)
        /// </summary>
        public int TabToSpace
        {
            get => _TabToSpace;
            set
            {
                if (_TabToSpace == value)
                    return;
                if (value < 1)
                    _TabToSpace = 1;
                else if (value > 10)
                    _TabToSpace = 10;
                else
                    _TabToSpace = value;
                LayoutChange?.Invoke(LayoutChangeProperty.TabToSpace);
            }
        }

        private SizeF _PageSize = new(210, 297);//A4大小
        /// <summary>
        /// 页面信息
        /// </summary>
        public SizeF PageSize
        {
            get => _PageSize;
            set
            {
                if (_PageSize == value)
                    return;

                _PageSize = value;
                LayoutChange?.Invoke(LayoutChangeProperty.PageSize);
            }
        }

        /// <summary>
        /// 客户区域大小
        /// </summary>
        public SizeF ClientSize => new SizeF(PageSize.Width - (Padding.Left + Padding.Right),PageSize.Height - (Padding.Top + Padding.Bottom));

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
