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
        /// 插入元素
        /// </summary>
        /// <param name="index">插入位置的索引</param>
        /// <param name="item">内容元素</param>
        public void InsertItem(int index, IContentItem item)
        {
            ContentItems.Insert(index,item);
        }

        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">插入位置的索引</param>
        /// <param name="items">内容元素集合</param>
        public void InsertRangeItem(int index, IEnumerable<IContentItem> items)
        {
            ContentItems.InsertRange(index, items);
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <param name="item">内容元素</param>
        public void AddItem(IContentItem item)
        {
            ContentItems.Add(item);
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <param name="items">内容元素集合</param>
        public void AddRangeItem(IEnumerable<IContentItem> items)
        {
            ContentItems.AddRange(items);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            ContentItems.Dispose();
        }
    }
}
