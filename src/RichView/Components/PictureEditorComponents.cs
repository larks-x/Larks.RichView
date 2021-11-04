namespace RichView.Components
{
    /// <summary>
    /// 图片编辑器组件(负责调整图片大小,旋转角度等)
    /// </summary>
    internal class PictureEditorComponents
    {
        public Action<ChangeSizeOption> ChangeSize;
        public class ChangeSizeOption{
            public int ItemNo { get; set; } = -1;
            public SizeF NewSize { get; set; } = new();
            public RectangleF ItemRectangle { get; set; } = new();
        }
        /// <summary>
        /// 绑定的ItemNo
        /// </summary>
        private int BindItemNo = -1;
        /// <summary>
        /// 绑定的图片
        /// </summary>
        private Image? BindImage = null;
        /// <summary>
        /// 边框画笔
        /// </summary>
        private Pen BorderPen = new Pen(Color.FromArgb(63, 63, 63));
        /// <summary>
        /// 锚点笔刷
        /// </summary>
        private Brush AnchorPointBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
        /// <summary>
        /// 锚点边框笔刷
        /// </summary>
        private Pen AnchorPointBorderBrush = new Pen(Color.FromArgb(178, 178, 178));
        /// <summary>
        /// 锚点绘制区域
        /// </summary>
        private List<AnchorPoint> AnchorPointRectangle { get; set; } = new List<AnchorPoint>();
        /// <summary>
        /// 鼠标上一次按下的坐标
        /// </summary>
        private PointF LastMousePoint = PointF.Empty;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool IsMouseDown { get; set; } = false;
        private AnchorPoint? HoverAnchorPoint;
        private object IsDraw = new object();
        /// <summary>
        /// 锚点半径
        /// </summary>
        public int AnchorPointRadius { get; set; } = 5;
        /// <summary>
        /// 编辑框区域
        /// </summary>
        public RectangleF EditorRectangle { get; set; } = RectangleF.Empty;
        /// <summary>
        /// 左上角坐标
        /// </summary>
        public PointF Location => EditorRectangle.Location;
        /// <summary>
        /// 大小
        /// </summary>
        public SizeF Size => EditorRectangle.Size;

        /// <summary>
        /// 是否已经绑定到图片
        /// </summary>
        public bool IsBind { get; private set; }
        private ImageAttributes ImageAtt;
        /// <summary>
        /// 图片透明度(0-1)
        /// </summary>
        public float ImageAlpha { get; set; } = 0.5f;

        public PictureEditorComponents()
        {
            float[][] matrixItems ={
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0,0 , 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, ImageAlpha, 0},
            new float[] {0, 0, 0, 0, 1}};
            ColorMatrix ColorMatrix = new ColorMatrix(matrixItems);
            ImageAtt = new ImageAttributes();
            ImageAtt.SetColorMatrix(
                ColorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
        }

        /// <summary>
        /// 绑定图片元素
        /// </summary>
        /// <param name="item"></param>
        public void Bind(HoverItem item)
        {
            if (item.InItem.ItemType != ItemType.Image)
                return;
            if (BindItemNo == item.ItemNo)
                return;
            if (BindItemNo != -1)
                UnBind();
            ImageItem imgItem = (ImageItem)item.InItem;
            LastMousePoint = Point.Empty;
            BindItemNo = item.ItemNo;
            if (imgItem.Image != null)
                BindImage = (Image)imgItem.Image.Clone();
            EditorRectangle = item.ItemRectangle;
            //不使用渐变笔刷
            //AnchorPointBrush = new LinearGradientBrush(new PointF(0, Size.Width / 2), new PointF(Size.Height, Size.Width / 2), Color.Red, Color.White);
            #region 锚点位置
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.TopLeft, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.TopCenter, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.TopRight, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.Left, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.Right, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.BottomLeft, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.BottomCenter, EditorRectangle, AnchorPointRadius));
            AnchorPointRectangle.Add(new AnchorPoint(AnchorPointType.BottomRight, EditorRectangle, AnchorPointRadius));

            #endregion
            IsBind = true;
        }

        /// <summary>
        /// 解绑图片元素
        /// </summary>
        public void UnBind()
        {
            BindItemNo = -1;
            BindImage?.Dispose();
            BindImage = null;
            AnchorPointRectangle.Clear();
            AnchorPointRectangle = new List<AnchorPoint>();
            IsBind = false;
        }

        /// <summary>
        /// 检查鼠标是否在图片编辑器上
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool MouseInPictureEditor(float x, float y)
        {
            // TODO: 如果要实现点击图片后还可以滑动鼠标选择其他Item, 就要在此方法内真实的验证鼠标是否在锚点上
            if (!IsBind)
                return false;
            if (EditorRectangle.Contains(x, y))
                return true;
            var hover = AnchorPointRectangle.Where(o => o.MouseHover(x, y)).Take(1).SingleOrDefault();
            if (hover != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 鼠标是否在编辑器中按下
        /// </summary>
        public bool EditorMouseDown => IsBind && IsMouseDown;   
            
        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OnLButtonDown(float x,float y)
        {
            Debug.WriteLine("鼠标按下");
            HoverAnchorPoint = AnchorPointRectangle.Where(o => o.MouseHover(x, y)).Take(1).SingleOrDefault();
            if (HoverAnchorPoint == null)
            {
                IsMouseDown = false;
                return;
            }
            LastMousePoint = new PointF(x, y);
            IsMouseDown = true;
            
        }

        /// <summary>
        /// 鼠标左键按下并滑动
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OnLButtonMove(float x, float y)
        {
            if (!IsMouseDown || HoverAnchorPoint is null)
                return;
            lock (IsDraw)
            {
                var diffX = x - LastMousePoint.X;
                var diffY = y - LastMousePoint.Y;
                LastMousePoint = new PointF(x,y);
                var oldRect = EditorRectangle;
                switch (HoverAnchorPoint.Type)
                {
                    case AnchorPointType.TopLeft:
                        EditorRectangle = new RectangleF(oldRect.X + diffX, oldRect.Y + diffY, oldRect.Width - diffX, oldRect.Height - diffY);
                        break;
                    case AnchorPointType.TopCenter:
                        EditorRectangle = new RectangleF(oldRect.X , oldRect.Y + diffY, oldRect.Width, oldRect.Height - diffY);
                        break;
                    case AnchorPointType.TopRight:
                        EditorRectangle = new RectangleF(oldRect.X , oldRect.Y + diffY, oldRect.Width + diffX, oldRect.Height - diffY);
                        break;
                    case AnchorPointType.Left:
                        EditorRectangle = new RectangleF(oldRect.X + diffX, oldRect.Y , oldRect.Width - diffX, oldRect.Height);
                        break;
                    case AnchorPointType.Right:
                        EditorRectangle = new RectangleF(oldRect.X , oldRect.Y , oldRect.Width + diffX , oldRect.Height);
                        break;
                    case AnchorPointType.BottomLeft:
                        EditorRectangle = new RectangleF(oldRect.X + diffX, oldRect.Y , oldRect.Width - diffX, oldRect.Height + diffY);
                        break;
                    case AnchorPointType.BottomCenter:
                        EditorRectangle = new RectangleF(oldRect.X , oldRect.Y , oldRect.Width , oldRect.Height + diffY);
                        break;
                    case AnchorPointType.BottomRight:
                        EditorRectangle = new RectangleF(oldRect.X , oldRect.Y, oldRect.Width + diffX, oldRect.Height + diffY);
                        break;
                }
                foreach (var ap in AnchorPointRectangle)
                {
                    ap.ChangeRectangle(EditorRectangle);
                }
            }
              
            //EditorRectangle
        }

        /// <summary>
        /// 鼠标左键弹起
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OnLButtonUp(float x, float y)
        {
            if (!IsMouseDown || HoverAnchorPoint is null)
                return;
            Debug.WriteLine("鼠标弹起@@@@");
            IsMouseDown = false;
            if (ChangeSize != null)
            {
                ChangeSizeOption option = new ChangeSizeOption()
                {
                    ItemNo = BindItemNo,
                    NewSize= EditorRectangle.Size,
                };
                ChangeSize(option);
                lock (IsDraw)
                {
                    EditorRectangle = option.ItemRectangle;
                    foreach (var ap in AnchorPointRectangle)
                    {
                        ap.ChangeRectangle(EditorRectangle);
                    }
                }
            }
            
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(Graphics graphics)
        {
            if (!IsBind)
                return;
            
            lock (IsDraw)
            {
                if (BindImage != null)
                    graphics.DrawImage(BindImage, EditorRectangle.ToRectangle(), 0,0, BindImage.Width, BindImage.Height, GraphicsUnit.Pixel, ImageAtt);
                graphics.DrawRectangle(BorderPen, Location.X, Location.Y, Size.Width, Size.Height);
                foreach (var ap in AnchorPointRectangle)
                {
                    graphics.FillEllipse(AnchorPointBrush, ap.Rectangle);
                    graphics.DrawEllipse(AnchorPointBorderBrush, ap.Rectangle);
                }
            }

        }
    }
}
