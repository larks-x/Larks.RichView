using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 锚点
    /// </summary>
    public class AnchorPoint
    {
        /// <summary>
        /// 锚点
        /// </summary>
        /// <param name="type">锚点类型</param>
        /// <param name="bindRect">绑定区域</param>
        /// <param name="anchorPointRadius">锚点半径</param>
        public AnchorPoint(AnchorPointType type,RectangleF bindRect,int anchorPointRadius)
        {
            AnchorPointRadius = anchorPointRadius;
            Type = type;
            BindRectangle = bindRect;
            BuildAnchorPoint();
        }
        /// <summary>
        /// 绑定区域
        /// </summary>
        public RectangleF BindRectangle { get; private set; }
        /// <summary>
        /// 锚点半径
        /// </summary>
        public int AnchorPointRadius { get; private set; }
        /// <summary>
        /// 锚点类型
        /// </summary>
        public AnchorPointType Type { get; set; }

        /// <summary>
        /// 锚点区域
        /// </summary>
        public RectangleF Rectangle { get; set; }
        /// <summary>
        /// 鼠标是否在这个锚点上
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool MouseHover(float x,float y)
        {
            return Rectangle.Contains(x,y);
        }

        /// <summary>
        /// 修改绑定区域
        /// </summary>
        /// <param name="bindRect"></param>
        public void ChangeRectangle(RectangleF bindRect)
        {
            BindRectangle = bindRect;
            BuildAnchorPoint();
        }

        /// <summary>
        /// 构造锚点
        /// </summary>
        private void BuildAnchorPoint()
        {
            switch (Type)
            {
                case AnchorPointType.TopLeft:
                    Rectangle = new RectangleF(BindRectangle.X - AnchorPointRadius, BindRectangle.Y - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.TopCenter:
                    Rectangle = new RectangleF(BindRectangle.X + (BindRectangle.Width - AnchorPointRadius * 2) / 2, BindRectangle.Y - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.TopRight:
                    Rectangle = new RectangleF(BindRectangle.Right - AnchorPointRadius, BindRectangle.Y - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.Left:
                    Rectangle = new RectangleF(BindRectangle.X - AnchorPointRadius, BindRectangle.Y + (BindRectangle.Height - AnchorPointRadius * 2) / 2, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.Center:

                    break;
                case AnchorPointType.Right:
                    Rectangle = new RectangleF(BindRectangle.Right - AnchorPointRadius, BindRectangle.Y + (BindRectangle.Height - AnchorPointRadius * 2) / 2, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.BottomLeft:
                    Rectangle = new RectangleF(BindRectangle.X - AnchorPointRadius, BindRectangle.Bottom - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.BottomCenter:
                    Rectangle = new RectangleF(BindRectangle.X + (BindRectangle.Width - AnchorPointRadius * 2) / 2, BindRectangle.Bottom - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;
                case AnchorPointType.BottomRight:
                    Rectangle = new RectangleF(BindRectangle.Right - AnchorPointRadius, BindRectangle.Bottom - AnchorPointRadius, AnchorPointRadius * 2, AnchorPointRadius * 2);
                    break;

            }

        }
    }
}
