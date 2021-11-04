using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 文本元素
    /// </summary>
    public class TextItem : ViewItem
    {
        /// <summary>
        /// 回车符
        /// </summary>
        public static TextItem Enter
        {
            get
            {
                var t = new TextItem()
                {
                    Text = Environment.NewLine,
                    IsControlKey = true,
                    Key = ControlKey.Enter
                };
                return t;
            }
        }

        /// <summary>
        /// 空格符
        /// </summary>
        public static TextItem Space
        {
            get
            {
                var t = new TextItem()
                {
                    Text = " ",
                    IsControlKey = true,
                    Key = ControlKey.Space
                };
                return t;
            }
        }

        /// <summary>
        /// Tab符
        /// </summary>
        public static TextItem Tab
        {
            get
            {
                var t = new TextItem()
                {
                    Text = "  ",
                    IsControlKey = true,
                    Key = ControlKey.Tab
                };
                return t;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextItem() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="styleNo"></param>
        public TextItem(string text,int styleNo)
        { 
            Text = text;
            StyleNo = styleNo;
        }

        /// <summary>
        /// 绘制大小
        /// </summary>
        public override SizeF DrawSize => Size;


        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object Clone()
        {
            return new TextItem()
            {
                Text = this.Text,
                StyleNo = this.StyleNo,
                LineNo = this.LineNo,
                IsSelect = this.IsSelect,
                IsControlKey = this.IsControlKey,
                Key = this.Key,
                Location = this.Location,
                Size = this.Size,
            };
        }

       

    }
}
