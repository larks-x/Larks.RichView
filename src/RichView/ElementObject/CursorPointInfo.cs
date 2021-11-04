namespace RichView.ElementObject
{
    /// <summary>
    /// 光标信息
    /// </summary>
    internal struct CursorPointInfo
    {
        /// <summary>
        /// 空的值
        /// </summary>
        public static CursorPointInfo Empty = new CursorPointInfo(0, 0, 0, 0);
        /// <summary>
        /// 光标信息
        /// </summary>
        /// <param name="l">Left</param>
        /// <param name="t">Top</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public CursorPointInfo(float l, float t, float w, float h)
        {
            Left = l;
            Top = t;
            Width = w;
            Height = h;
        }
        /// <summary>
        /// 左边距
        /// </summary>
        public float Left { get; set; }
        /// <summary>
        /// 上边距
        /// </summary>
        public float Top { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// 计算对象是否相等
        /// </summary>
        /// <param name="si1"></param>
        /// <param name="si2"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool operator ==(CursorPointInfo si1, CursorPointInfo si2)
        {
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
        public static bool operator !=(CursorPointInfo si1, CursorPointInfo si2)
        {
            if (!si1.Equals(si2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证一个对象是否和此实例相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
                return false;
            CursorPointInfo other = (CursorPointInfo)obj;
            if (Top == other.Top && Left == other.Left && Width == other.Width && Height == other.Height)
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
            return Top.GetHashCode() + Left.GetHashCode()+ Width.GetHashCode()+ Height.GetHashCode();
        }
    }
}
