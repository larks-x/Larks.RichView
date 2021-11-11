using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// 文本Item
    /// </summary>
    public class TextItem:ContentItem
    {
        /// <summary>
        /// Tab
        /// </summary>
        public static TextItem Tab => new TextItem("    ",null, ControlKey.Tab);
        /// <summary>
        /// 空格
        /// </summary>
        public static TextItem Space => new TextItem(" ",null, ControlKey.Space);
        /// <summary>
        /// 换行符
        /// </summary>
        public static TextItem Enter => new TextItem(Environment.NewLine, "↵", ControlKey.Enter);
        /// <summary>
        /// Item类型
        /// </summary>
        public override ItemType ItemType => ItemType.Text;
        /// <summary>
        /// 绘制大小
        /// </summary>
        public override SizeF DrawSize => Size;
        /// <summary>
        /// 绘制文本
        /// </summary>
        [JsonIgnore]
        public override string DrawText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TextItem(){}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public TextItem(string text)
        { 
            Text = text;
            DrawText = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="drawText"></param>
        /// <param name="key"></param>
        public TextItem(string text,string drawText = null, ControlKey key = ControlKey.None)
        {
            Text = text;
            DrawText = text;
            if (key != ControlKey.None)
            {
                if(drawText!=null)
                    DrawText = drawText;
                IsControlKey = true;
                Key = key;
            }
        }
    }
}
