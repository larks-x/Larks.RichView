namespace Larks.RichView.ContentElements
{
    public class TextContent : ContentItem
    {
        /// <summary>
        /// 回车符
        /// </summary>
        public static TextContent Enter
        {
            get
            {
                var t = new TextContent()
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
        public static TextContent Space
        {
            get
            {
                var t = new TextContent()
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
        public static TextContent Tab
        {
            get
            {
                var t = new TextContent()
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
        public TextContent()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="styleNo"></param>
        public TextContent(string text, int styleNo)
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
            return new TextContent()
            {
                Text = this.Text,
                StyleNo = this.StyleNo,
                Line = this.Line,
                IsSelect = this.IsSelect,
                IsControlKey = this.IsControlKey,
                Key = this.Key,
                Location = this.Location,
                Size = this.Size,
            };
        }
    }
}
