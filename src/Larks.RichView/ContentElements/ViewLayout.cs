using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// 页面布局
    /// </summary>
    public class ViewLayout
    {
        /// <summary>
        /// 内边距
        /// </summary>
        public Padding Padding { get; set; } = new Padding(3);

        /// <summary>
        /// 行间距(介于0-20之间)
        /// </summary>
        public int RowSpacing { get; set; } = 4;

        /// <summary>
        /// 一个Tab等于几个空格(介于1-10之间)
        /// </summary>
        public int TabToSpace { get; set; } = 2;

        /// <summary>
        /// 页面信息
        /// </summary>
        public SizeF PageSize { get; set; } = new(210, 297);//A4大小
    }
}
