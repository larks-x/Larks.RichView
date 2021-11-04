namespace RichView.ElementObject
{
    /// <summary>
    /// 段落信息
    /// </summary>
    public class ParagraphInfo:ICloneable
    {
        /// <summary>
        /// 默认段落设置
        /// </summary>
        public static ParagraphInfo Default = new();

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public ParagraphInfo Copy()
        {
            return new ParagraphInfo(ParagraphNo, Alignment);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return Copy();
        }

        /// <summary>
        /// 
        /// </summary>
        public ParagraphInfo()
        {
            ParagraphNo = 0;
            Alignment = ParagraphAlignment.Left;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraphNo"></param>
        /// <param name="paragraphAlignment"></param>
        public ParagraphInfo(int paragraphNo, ParagraphAlignment paragraphAlignment)
        {
            ParagraphNo = paragraphNo;
            Alignment = paragraphAlignment;
        }
        /// <summary>
        /// 段落编号
        /// </summary>
        public int ParagraphNo { get; set; }
        /// <summary>
        /// 段落对齐
        /// </summary>
        public ParagraphAlignment Alignment { get; set; }
    }
}
