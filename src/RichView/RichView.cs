namespace RichView
{
    /// <summary>
    /// RichViewControl
    /// </summary>
    public partial class RichView : UserControl
    {
        #region Public
        /// <summary>
        /// 图片编辑器锚点半径(介于5-10之间)
        /// </summary>
        public int PictureAnchorPointRadius
        {
            get => PictureEditor.AnchorPointRadius;
            set {
                if (PictureEditor.AnchorPointRadius == value)
                    return;
                if (value <= 0)
                    PictureEditor.AnchorPointRadius = 5;
                if (value >= 10)
                    PictureEditor.AnchorPointRadius = 10;
                PictureEditor.AnchorPointRadius = value;
            }
        } 

        private long _CursorIndex = 0;
        /// <summary>
        /// 光标位置
        /// </summary>
        [Browsable(false)]
        public long CursorIndex
        {
            get => _CursorIndex;
            set {
                if (_CursorIndex == value)
                    return;
                if (value < -1)
                    _CursorIndex = -1;
                else if (value > ContextItems.Count)
                    _CursorIndex = ContextItems.Count;
                else
                    _CursorIndex = value;
            }
        }

        /// <summary>
        /// 内间距
        /// </summary>
        public new Padding Padding
        {
            get => ContextViewInfo.Layout.Padding;
            set {
                if (ContextViewInfo.Layout.Padding == value)
                    return;
                ContextViewInfo.Layout.Padding = value;
                Refresh();
            }
        }

        /// <summary>
        /// 行间距(介于0-100之间)
        /// </summary>
        public int RowSpacing
        {
            get => ContextViewInfo.Layout.RowSpacing;
            set {
                if (ContextViewInfo.Layout.RowSpacing == value)
                    return;
                if (value < 0)
                    ContextViewInfo.Layout.RowSpacing = 0;
                else if (value > 100)
                    ContextViewInfo.Layout.RowSpacing = 100;
                else
                    ContextViewInfo.Layout.RowSpacing = value;
                Refresh();
            }
        }

        /// <summary>
        /// 一个Tab等于几个空格(介于1-10之间)
        /// </summary>
        [Description("一个Tab等于几个空格(介于1-10之间)")]
        public int TabToSpace
        {
            get
            {
                return ContextViewInfo.Layout.TabToSpace;
            }
            set
            {
                if (ContextViewInfo.Layout.TabToSpace == value)
                    return;
                if (value < 1)
                    ContextViewInfo.Layout.TabToSpace = 1;
                else if (value > 10)
                    ContextViewInfo.Layout.TabToSpace = 10;
                else
                    ContextViewInfo.Layout.TabToSpace = value;
                Refresh();
            }
        }

        /// <summary>
        /// 是否有文本处于选中状态
        /// </summary>
        [Browsable(false)]
        public bool TextSelected
        {
            get
            {
                if (ContextItems.Count == 0)
                    return false;
                var list = ContextItems.Where(o => o.IsSelect).ToList();
                if (list == null || list.Count == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 画布
        /// </summary>
        [Browsable(false)]
        public Graphics Graphics { get; private set; }

        #endregion

        /// <summary>
        /// RichViewControl
        /// </summary>
        public RichView()
        {
            InitializeComponent();

            if (IsDesignMode())
                return;
            GCThread = new Thread(GCThreadMethod)
            {
                IsBackground = true
            };
            BlendThread = new Thread(UIThreadMethod)
            {
                IsBackground = true
            };
           
            DrawMiddleLayerThread = new Thread(DrawMiddleLayer)
            {
                IsBackground = true
            };

            //DoubleBuffer双缓冲，AllPaintingInWmPaint禁止擦除背景
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw
            | ControlStyles.SupportsTransparentBackColor
            , true);
            this.UpdateStyles();
            Init();
            IME = new ImeComponent(this);
            IME.InputText += (s) =>
            {
                ProcessingIMEInput(s);
            };
            IsInitialization = true;
            GCThread.Start();
            DrawMiddleLayerThread.Start();
            BlendThread.Start();
        }

        /// <summary>
        /// 应用新的样式
        /// </summary>
        /// <param name="font">字体</param>
        /// <param name="color">字体颜色</param>
        public void ApplyStyle(Font font, Color? color = null)
        {
            ApplyStyle(font.Name,font.Size, font.Style, color);
        }

        /// <summary>
        /// 应用新的样式
        /// </summary>
        /// <param name="fontName">字体名称</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="style">字体风格</param>
        /// <param name="color">字体颜色</param>
        public void ApplyStyle(string fontName,float? fontSize = null, FontStyle? style = null,Color? color = null)
        {
            var sl = SelectedItem();
            if (sl == null || sl.Count == 0)
                return;
            var f = sl.First();
            var curStyle = GetStyleInfo(f.StyleNo);
            var newStyle= curStyle.Copy();
            newStyle.StyleNo = GetNewStyleNo();
            if (style == null)
                style = curStyle.StyleFont.Style;
            if (fontSize == null)
                fontSize = curStyle.StyleFont.Size;
            newStyle.StyleFont = new Font(fontName, fontSize.Value, style.Value);

            if (color != null)
                newStyle.FontColor = color.Value;
            StyleInfos.Add(newStyle);
            foreach (var t in sl)
                t.StyleNo = newStyle.StyleNo;
            MeasureItems(0);
        }

        /// <summary>
        /// 在当前位置插入图片
        /// </summary>
        /// <param name="img"></param>
        public void InsertImage(Image img)
        {
            ContextItems.Insert((int)CursorIndex, new ImageItem(img));
            MeasureItems(0);
        }

        /// <summary>
        /// 在当前位置插入图片
        /// </summary>
        /// <param name="bitmap"></param>
        public void InsertImage(Bitmap bitmap)
        {
            InsertImage((Image)bitmap);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public override void Refresh()
        {
            MeasureItems(0);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (UndoStack.Count == 0)
                return;
            var cmd = UndoStack.Pop();
            cmd.Undo();
            AddRedo(cmd);
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Redo()
        {
            if (RedoStack.Count == 0)
                return;
            var cmd = RedoStack.Pop();
            cmd.Redo();

            AddUndo(cmd);
        }

        /// <summary>
        /// 所有内容的文本表达方式
        /// </summary>
        public override string Text => AllString();

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.txt|*.txt";
            if (sd.ShowDialog() == DialogResult.Cancel)
                return;

            var path = sd.FileName;

            string json = string.Empty;
            
#if NET5_0 || NET6_0
            json = JsonSerializer.Serialize(ContextViewInfo);
#else
            json = JsonConvert.SerializeObject(ContextViewInfo);
#endif
            System.IO.File.WriteAllText(path, json);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public void LoadFile()
        { 
            OpenFileDialog of=new OpenFileDialog();
            of.Filter = "*.txt|*.txt";
            if (of.ShowDialog() == DialogResult.Cancel)
                return;
            var path = of.FileName;
            string s = System.IO.File.ReadAllText(path);
            ViewInfo? obj;
#if NET5_0 || NET6_0
            obj = JsonSerializer.Deserialize<ViewInfo>(s);
#else
            obj = JsonConvert.DeserializeObject<ViewInfo>(s);
#endif
            ContextViewInfo = obj;
            Refresh();
        }

        public int ItemCount()
        {
            return ContextItems.Count;
        }
    }
}