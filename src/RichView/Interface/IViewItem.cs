﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.Interface
{
    interface IViewItem : ICloneable,IDisposable
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        long UniqueKey { get; }
        /// <summary>
        /// Item类型
        /// </summary>
        ItemType ItemType { get; }
        /// <summary>
        /// Item文本
        /// </summary>
        string Text { get; }
        /// <summary>
        /// 文本风格
        /// </summary>
        int StyleNo { get; set; }
        /// <summary>
        /// 所在行号
        /// </summary>
        int LineNo { get; set; }
        /// <summary>
        /// 是否为选中状态
        /// </summary>
        bool IsSelect { get; set; }
        /// <summary>
        /// 是否为控制符
        /// </summary>
        bool IsControlKey { get; }
        /// <summary>
        /// 控制符
        /// </summary>
        ControlKey Key { get; }
        /// <summary>
        /// 绘制坐标
        /// </summary>
        PointF Location { get; set; }
        /// <summary>
        /// 元素原始大小
        /// </summary>
        SizeF Size { get; set; }
        /// <summary>
        /// 元素缩放绘制的大小(对于Image类型的元素需要在此实现缩放)
        /// </summary>
        SizeF DrawSize { get; }
        /// <summary>
        /// 绘制区域
        /// </summary>
        /// <returns></returns>
        RectangleF DrawRectangle { get; }
        /// <summary>
        /// 测量元素并返回元素的绘制区域
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="viewInfo">View信息</param>
        /// <returns></returns>
        RectangleF Measure(Graphics graphics, ViewInfo viewInfo);
        /// <summary>
        /// 尝试追加一行
        /// </summary>
        /// <param name="viewInfo"></param>
        /// <param name="lineNo">行号</param>
        void TryAddLine(ViewInfo viewInfo, int lineNo);
        /// <summary>
        /// 行高发生变化时触发
        /// </summary>
        /// <param name="lineInfo">行信息</param>
        void LineHeightChange(LineInfo lineInfo);
        /// <summary>
        /// 指定点在元素的哪个区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        MouseInItem PointInItem(float x, float y);
        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="viewInfo">View信息</param>
        void Draw(Graphics graphics, ViewInfo viewInfo);
       
    }
}