namespace RichView.Enums
{
    /// <summary>
    /// 控制键
    /// </summary>
    public enum ControlKey
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Enter
        /// </summary>
        Enter = 13,
        /// <summary>
        /// Tab
        /// </summary>
        Tab = 9,
        /// <summary>
        /// Backspace
        /// </summary>
        Backspace = 8,
        /// <summary>
        /// Del
        /// </summary>
        Delete = 46,
        /// <summary>
        /// Clear
        /// </summary>
        Clear = 12,
        /// <summary>
        /// Shift
        /// </summary>
        Shift = 16,
        /// <summary>
        /// Ctrl
        /// </summary>
        Control = 17,
        /// <summary>
        /// Alt
        /// </summary>
        Alt = 18,
        /// <summary>
        /// Esc
        /// </summary>
        Esc = 27,
        /// <summary>
        /// Space
        /// </summary>
        Space = 32,
        /// <summary>
        /// PageUp
        /// </summary>
        PageUp = 33,
        /// <summary>
        /// PageDown
        /// </summary>
        PageDown = 34,
        /// <summary>
        /// End
        /// </summary>
        End = 35,
        /// <summary>
        /// Home
        /// </summary>
        Home = 36,
        /// <summary>
        /// LeftArrow
        /// </summary>
        LeftArrow = 37,
        /// <summary>
        /// UpArrow
        /// </summary>
        UpArrow = 38,
        /// <summary>
        /// RightArrow
        /// </summary>
        RightArrow = 39,
        /// <summary>
        /// DownArrow
        /// </summary>
        DownArrow = 40,
        /// <summary>
        /// F1
        /// </summary>
        F1 = 112,
        /// <summary>
        /// F2
        /// </summary>
        F2 = 113,
        /// <summary>
        /// F3
        /// </summary>
        F3 = 114,
        /// <summary>
        /// F4
        /// </summary>
        F4 = 115,
        /// <summary>
        /// F5
        /// </summary>
        F5 = 116,
        /// <summary>
        /// F6
        /// </summary>
        F6 = 117,
        /// <summary>
        /// F7
        /// </summary>
        F7 = 118,
        /// <summary>
        /// F8
        /// </summary>
        F8 = 119,
        /// <summary>
        /// F9
        /// </summary>
        F9 = 120,
        /// <summary>
        /// F10
        /// </summary>
        F10 = 121,
        /// <summary>
        /// F11
        /// </summary>
        F11 = 122,
        /// <summary>
        /// F12
        /// </summary>
        F12 = 123,
    }

    /// <summary>
    /// 鼠标在Item的哪个位置
    /// </summary>
    public enum MouseInItem
    {
        /// <summary>
        /// 不在Item上
        /// </summary>
        No = 0,
        /// <summary>
        /// 在Item的前半部分
        /// </summary>
        Befor = 1,
        /// <summary>
        /// 在Item的后半部分
        /// </summary>
        After = 2
    }

    /// <summary>
    /// 元素类型
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text = 0,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 1,
        /// <summary>
        /// 表格
        /// </summary>
        Table = 2,
    }

    /// <summary>
    /// 段落对其方式
    /// </summary>
    public enum ParagraphAlignment
    { 
        /// <summary>
        /// 无段落样式
        /// </summary>
        None = 0,
        /// <summary>
        /// 左对齐
        /// </summary>
        Left = 1,
        /// <summary>
        /// 右对齐
        /// </summary>
        Right = 2,
        /// <summary>
        /// 两端对齐(暂不实现)
        /// </summary>
        Justified = 3,
        /// <summary>
        /// 分散对齐(暂不实现)
        /// </summary>
        Distributed = 4,
    }

    /// <summary>
    /// 锚点类型
    /// </summary>
    public enum AnchorPointType
    { 
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft=0,
        /// <summary>
        /// 中上
        /// </summary>
        TopCenter=1,
        /// <summary>
        /// 右上
        /// </summary>
        TopRight=2,
        /// <summary>
        /// 左
        /// </summary>
        Left=3,
        /// <summary>
        /// 中
        /// </summary>
        Center=4,
        /// <summary>
        /// 右
        /// </summary>
        Right=5,
        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft=6,
        /// <summary>
        /// 中下
        /// </summary>
        BottomCenter=7,
        /// <summary>
        /// 右下
        /// </summary>
        BottomRight=8,
    }

    /// <summary>
    /// Layout属性变化
    /// </summary>
    public enum LayoutChangeProperty
    {
        /// <summary>
        /// Padding变化
        /// </summary>
        Padding = 0,
        /// <summary>
        /// RowSpacing变化
        /// </summary>
        RowSpacing = 1,
        /// <summary>
        /// TabToSpace变化
        /// </summary>
        TabToSpace = 2,
        /// <summary>
        /// PageSize变化
        /// </summary>
        PageSize = 3,

    }
}
