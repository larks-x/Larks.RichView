using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// RichView页面信息
    /// </summary>
    public class RichViewInformation:IDisposable
    {
        /// <summary>
        /// 光标所在的索引
        /// </summary>
        public int CursorIndex { get; internal set; } = -1;

        /// <summary>
        /// 布局信息
        /// </summary>
        public ViewLayout Layout = new();
        
        /// <summary>
        /// RichView的元素
        /// </summary>
        public ContainerList<IContentItem> ContentItems = new();


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            ContentItems.Dispose();
        }
    }
}
