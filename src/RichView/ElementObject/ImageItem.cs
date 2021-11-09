using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageItem : ViewItem
    {
        /// <summary>
        /// 
        /// </summary>
        public ImageItem()
        {
            Text = "[Image]";
        }

        /// <summary>
        /// 
        /// </summary>
        public ImageItem(Image image)
        {
            Text = "[Image]";
            ImageSource = ConvertBase64(image);
            if (Image != null)
            {
                Size = image.Size;
                DrawSize = Size;
            }
        }

        /// <summary>
        /// Item类型
        /// </summary>
        public override ItemType ItemType => ItemType.Image;

        /// <summary>
        /// 是否为控制键
        /// </summary>
        public override bool IsControlKey => false;
        /// <summary>
        /// 控制键
        /// </summary>
        public override ControlKey Key => ControlKey.None;

        /// <summary>
        /// 绘制大小
        /// </summary>
        public override SizeF DrawSize { get; set; }

        private string _ImageSource = string.Empty;
        /// <summary>
        /// 图片数据(图片的Base64数据)
        /// </summary>
        public string ImageSource
        {
            get => _ImageSource;
            set
            {
                if (_ImageSource == value)
                    return;
                this.Image?.Dispose();
                this.Image = null;
                _ImageSource = value;
                Image = ConvertToImage(_ImageSource);
            }
        }

        /// <summary>
        /// ImageSource转换为Image
        /// </summary>
        [JsonIgnore]
        public Image? Image { get; private set; }

        /// <summary>
        /// 测量元素
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="viewInfo">ViewInfo</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override RectangleF Measure(Graphics graphics, ViewInfo viewInfo)
        {
            RectangleF previousItemRect = RectangleF.Empty;
            int curLineNo = 0;
            var lastItemIndex = viewInfo.ContextItems.IndexOf(this) - 1;
            if (lastItemIndex < 0)
                previousItemRect = new RectangleF(viewInfo.Layout.Padding.Left, 0, 0, 0);
            else
            {
                previousItemRect = viewInfo.ContextItems[lastItemIndex].DrawRectangle;
                curLineNo = viewInfo.ContextItems[lastItemIndex].LineNo;
            }
            LineNo = curLineNo;
            Location = new PointF(previousItemRect.Right, viewInfo.LineInfos[LineNo].Top);
            if (DrawRectangle.Right > viewInfo.LineInfos[LineNo].Right || Key == ControlKey.Enter)
            {
                LineNo++;
                TryAddLine(viewInfo, LineNo);
                Location = new PointF(viewInfo.Layout.Padding.Left, viewInfo.LineInfos[LineNo].Top);
            }
            return DrawRectangle;
        }

        /// <summary>
        /// 行高发生变化时触发
        /// </summary>
        /// <param name="lineInfo">行信息</param>
        public override void LineHeightChange(LineInfo lineInfo)
        {
            Location = new PointF(Location.X, lineInfo.Bottom - DrawSize.Height);
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="graphics">画布</param>
        /// <param name="viewInfo">ViewInfo</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Draw(Graphics graphics, ViewInfo viewInfo)
        {
            if (Image == null)
                return;
            graphics.DrawImage(this.Image, DrawRectangle, new RectangleF(0, 0, Size.Width,Size.Height), GraphicsUnit.Pixel);
        }


        /// <summary>
        /// base64转到Image
        /// </summary>
        /// <returns></returns>
        private Image? ConvertToImage(string base64Str)
        {
            this.Image?.Dispose();
            this.Image = null;
            if (string.IsNullOrEmpty(base64Str))
                return null;
            try
            {
                byte[] b = Convert.FromBase64String(base64Str);
                var ms = new System.IO.MemoryStream(b);
                var bitmap = new Bitmap(ms);
                return (Image)bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 图片转换到Base64
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private string ConvertBase64(Image image)
        {
            if (image == null)
                return string.Empty;
            string strbaser64 = string.Empty;
            try
            {
                var bitMap = new Bitmap(image);
                using (var ms = new System.IO.MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();

                    strbaser64 = Convert.ToBase64String(arr);
                }
            }
            catch
            {

            }
            return strbaser64;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object Clone()
        {
            return new ImageItem()
            {
                Text = this.Text,
                StyleNo = this.StyleNo,
                LineNo = this.LineNo,
                IsSelect = this.IsSelect,
                Location = this.Location,
                ImageSource = this.ImageSource,
                DrawSize = this.DrawSize,
            };
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            this.Image?.Dispose();
            this.Image = null;
            base.Dispose();
        }
    }
}
