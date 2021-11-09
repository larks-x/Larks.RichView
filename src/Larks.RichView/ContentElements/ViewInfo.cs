namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// ViewInfo
    /// </summary>
    public class ViewInfo
    {
        /// <summary>
        /// 画布
        /// </summary>
        [Browsable(false)]
        public Graphics Graphics { get; internal set; }
        /// <summary>
        /// 页面大小改变
        /// </summary>
        public Action<Graphics,Size> OnChangePageSize;
        internal void InvokeOnChangePageSize(Size size)
        {
            OnChangePageSize?.Invoke(Graphics, size);
        }
        /// <summary>
        /// 绘制事件
        /// </summary>
        public Action<Graphics> OnDraw;
        /// <summary>
        /// 触发绘制
        /// </summary>
        internal void InvokeOnDraw()
        {
            OnDraw?.Invoke(Graphics);
        }
        /// <summary>
        /// 样式列表
        /// </summary>
        public List<StyleInfo> StyleInfos { get; set; } = new();
        ///// <summary>
        ///// 段落样式
        ///// </summary>
        //public List<ParagraphInfo> ParagraphInfos { get; set; } = new();

        /// <summary>
        /// 所有行
        /// </summary>
        public List<LineContainer> Lines { get; set; } = new();
        /// <summary>
        /// 所有元素
        /// </summary>
        public List<ContentItem> ContextItems { get; set; } = new();
        /// <summary>
        /// View布局
        /// </summary>
        public LayoutInfo Layout { get; set; } = new();
        private DrawingManaged DM;
        public ViewInfo(DrawingManaged dm)
        {
            DM = dm;
            Layout.LayoutChange += (p) =>
            {
                if (p == LayoutChangeProperty.PageSize)
                {
                    InvokeOnChangePageSize(DM.Size);
                    InvokeOnDraw();
                }
            };
        }

        /// <summary>
        /// 光标所在行
        /// </summary>
        internal int CursorLine { get; private set; }
        /// <summary>
        /// 计算光标所在行
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void CalculationCursorLine(int x, int y)
        {
            var line = Lines.SingleOrDefault(o => o.MouseVerificationArea.Contains(x, y));
            if (line == null)
                CursorLine = Lines.Count - 1;
            else
                CursorLine = Lines.IndexOf(line);
            Debug.WriteLine($"当前光标所在行{CursorLine}");
        }

        internal void AddItems(List<ContentItem> items)
        {
            Lines[CursorLine].Contents.AddRange(items);
        }

        internal int MoveNextLine()
        {
            CursorLine++;
            return CursorLine;
        }

        /// <summary>
        /// 克隆此实例
        /// </summary>
        /// <returns></returns>
        public ViewInfo Clone()
        {
            var obj = new ViewInfo(DM)
            {
                Layout = (LayoutInfo)this.Layout.Clone(),
                StyleInfos = this.StyleInfos.Clone(),
                //ParagraphInfos = this.ParagraphInfos.Clone(),
                Lines = this.Lines.Clone(),
                ContextItems = this.ContextItems.Clone(),
            };
            return obj;
        }

        public void AddLine()
        {
            var tmp = new LineContainer(this);
            
            Lines.Add(tmp);
            tmp.DetermineLocation(true);
            InvokeOnDraw();
        }
    }
}
