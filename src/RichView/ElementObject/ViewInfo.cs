namespace RichView.ElementObject
{
    /// <summary>
    /// ViewInfo
    /// </summary>
    public class ViewInfo
    {
        /// <summary>
        /// 页面大小改变
        /// </summary>
        public Action ChangePageSize;
        /// <summary>
        /// 样式列表
        /// </summary>
        public List<StyleInfo> StyleInfos { get; set; } = new();
        /// <summary>
        /// 段落样式
        /// </summary>
        public List<ParagraphInfo> ParagraphInfos { get; set; } = new();
        /// <summary>
        /// 行信息
        /// </summary>
        public List<LineInfo> LineInfos { get; set; } = new();
        /// <summary>
        /// 所有行
        /// </summary>
        public List<LineContainer> Lines { get; set; } = new();
        /// <summary>
        /// 所有元素
        /// </summary>
        public List<ViewItem> ContextItems { get; set; } = new();
        /// <summary>
        /// View布局
        /// </summary>
        public LayoutInfo Layout { get; set; } = new();

        public ViewInfo()
        {
            Layout.LayoutChange += (p) =>
            {
                if(p == LayoutChangeProperty.PageSize)
                    ChangePageSize?.Invoke();
            };
        }

        /// <summary>
        /// 克隆此实例
        /// </summary>
        /// <returns></returns>
        public ViewInfo Clone()
        {
            var obj = new ViewInfo()
            {
                Layout = (LayoutInfo)this.Layout.Clone(),
                StyleInfos = this.StyleInfos.Clone(),
                ParagraphInfos = this.ParagraphInfos.Clone(),
                LineInfos = this.LineInfos.Clone(),
                Lines = this.Lines.Clone(),
                ContextItems = this.ContextItems.Clone(),
            };
            return obj;
        }
    }
}
