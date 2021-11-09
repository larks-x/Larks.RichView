using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.Components
{
    /// <summary>
    /// Table组件
    /// </summary>
    public class TableViewComponents
    {
        /// <summary>
        /// 列
        /// </summary>
        public int ColumnCount { get; private set; }
        /// <summary>
        /// 行
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// 边框宽度(2-5之间)
        /// </summary>
        public int BorderWidth { get; set; } = 2;

        /// <summary>
        /// Table组件
        /// </summary>
        /// <param name="colCount">列数</param>
        /// <param name="rowCount">行数</param>
        public TableViewComponents(int colCount,int rowCount)
        {
            ColumnCount = colCount;
            RowCount = rowCount;
        }
    }
}
