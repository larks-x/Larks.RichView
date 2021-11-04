namespace RichView.ElementObject
{
    /// <summary>
    /// 文本Item样式
    /// </summary>
    public class StyleInfo: ICloneable
    {
        /// <summary>
        /// 默认Style设置
        /// </summary>
        public static StyleInfo Default = new();
        /// <summary>
        /// 
        /// </summary>
        public StyleInfo() 
        {
            StyleNo = 0;
            StyleFont = new Font("宋体", 12);
            FontColor = Color.Black;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="styleNo"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public StyleInfo(int styleNo,Font font,Color color)
        {
            StyleNo = styleNo;
            StyleFont = font;
            FontColor = color;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="styleNo"></param>
        /// <param name="fontName"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        public StyleInfo(int styleNo, string fontName,int fontSize, Color color)
        {
            StyleNo = styleNo;
            StyleFont = new Font(fontName, fontSize);
            FontColor = color;
        }

        /// <summary>
        /// 拷贝一个副本
        /// </summary>
        /// <returns></returns>
        public StyleInfo Copy()
        {
            var newStyle = new StyleInfo()
            {
                StyleNo = -1,
                StyleFont = (Font)this.StyleFont.Clone(),
                FontColor = this.FontColor
            };


            return newStyle;
        }

        /// <summary>
        /// 样式编号(此字段不参与Equals计算,只用作内部标识)
        /// </summary>
        public int StyleNo { get; set; }

        /// <summary>
        /// 字体名称
        /// </summary>
        public string StyleFontName {
            get => StyleFont.Name;
            set {
                if (StyleFont.Name == value)
                    return;
                var newFont = new Font(value, StyleFont.Size, StyleFont.Style, StyleFont.Unit);
                StyleFont = newFont;
            }
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        public float StyleFontSize
        {
            get => StyleFont.Size;
            set
            {
                if (StyleFont.Size == value)
                    return;
                var newFont = new Font(StyleFont.Name, value, StyleFont.Style, StyleFont.Unit);
                StyleFont = newFont;
            }
        }

        /// <summary>
        /// 字体风格
        /// </summary>
        public FontStyle StyleFontStyle
        {
            get => StyleFont.Style;
            set
            {
                if (StyleFont.Style == value)
                    return;
                var newFont = new Font(StyleFont.Name, StyleFont.Size, value, StyleFont.Unit);
                StyleFont = newFont;
            }
        }

        /// <summary>
        /// 字体绘制单位
        /// </summary>
        public GraphicsUnit StyleFontUnit
        {
            get => StyleFont.Unit;
            set
            {
                if (StyleFont.Unit == value)
                    return;
                var newFont = new Font(StyleFont.Name, StyleFont.Size, StyleFont.Style, value);
                StyleFont = newFont;
            }
        }

        private Font _StyleFont;

        /// <summary>
        /// 字体
        /// </summary>
        [JsonIgnore]
        public Font StyleFont
        { 
            get => _StyleFont;
            set {
                if (_StyleFont == value)
                    return;
                _StyleFont = value;
            }
        }

        private Color _FontColor;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor
        {
            get => _FontColor;
            set {
                if (_FontColor == value)
                    return;
                _FontColor = value;
                if (_DrawBrush != null)
                    _DrawBrush.Dispose();
                _DrawBrush=new SolidBrush(_FontColor);
            }
        }

        private Brush _DrawBrush = null;
        /// <summary>
        /// 绘制笔刷
        /// </summary>
        internal Brush DrawBrush {
            get { 
                if(_DrawBrush ==null)
                    _DrawBrush = new SolidBrush(_FontColor);
                return _DrawBrush;
            } 
            private set {
                if (_DrawBrush == value)
                    return;
                _DrawBrush = value;
            } 
        }

        /// <summary>
        /// 计算对象是否相等
        /// </summary>
        /// <param name="si1"></param>
        /// <param name="si2"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool operator ==(StyleInfo? si1, StyleInfo? si2)
        {
            if (si1 is null && si2 is null)
                return true;
            if (si1 is null || si2 is null)
                return false;
            if (si1.Equals(si2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 计算对象是否不等
        /// </summary>
        /// <param name="si1"></param>
        /// <param name="si2"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool operator !=(StyleInfo? si1, StyleInfo? si2)
        {
            if (si1 == si2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 验证一个对象是否和此实例相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
                return false;
            StyleInfo other = (StyleInfo)obj;
            if (StyleFont.Equals(other.StyleFont) && FontColor.Equals(other.FontColor))
                return true;
            else
                return false;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return StyleFont.GetHashCode() + FontColor.GetHashCode();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return Copy();
        }
    }
}
