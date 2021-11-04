namespace RichView.Helpers
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>
        /// PointF转Point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point ToPoint(this PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        /// <summary>
        /// PointF转Point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PointF ToPointF(this Point p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        /// <summary>
        /// SizeF转Size
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Size ToSize(this SizeF s)
        {
            return new Size((int)s.Width , (int)s.Height);
        }

        /// <summary>
        /// SizeF转Size
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static SizeF ToSizeF(this Size s)
        {
            return new SizeF((float)s.Width, (float)s.Height);
        }

        /// <summary>
        /// RectangleF 转 PointF[]
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static PointF[] ToPointFArray(this RectangleF r)
        {
            List<PointF> list = new List<PointF>();
            list.Add(new PointF(r.X,r.Y));
            list.Add(new PointF(r.Right, r.Y));
            list.Add(new PointF(r.X, r.Bottom));
            return list.ToArray();
        }

        /// <summary>
        /// RectangleF 转 PointF[]
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(this RectangleF r)
        {

            return new Rectangle(r.Location.ToPoint(), r.Size.ToSize());
        }

        /// <summary>
        /// List克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
