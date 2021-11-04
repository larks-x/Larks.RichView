namespace RichView
{
    /// <summary>
    /// RichViewControl
    /// </summary>
    partial class RichView
    {
        #region Paivate
        /// <summary>
        /// 线程取消令牌,因为net35不支持CancellationToken,所以这里使用bool
        /// </summary>
        private bool CancellationToken = false;
        /// <summary>
        /// 内容绘制队列
        /// </summary>
        private Queue<int> DrawContextQueue=new Queue<int> ();
        /// <summary>
        /// 撤销栈
        /// </summary>
        private Stack<ICommand> UndoStack = new Stack<ICommand>();
        /// <summary>
        /// 重做栈
        /// </summary>
        private Stack<ICommand> RedoStack = new Stack<ICommand>();
        /// <summary>
        /// 合成线程
        /// </summary>
        private Thread BlendThread;

        /// <summary>
        /// GC线程
        /// </summary>
        private Thread GCThread;

        /// <summary>
        /// 中间层绘制线程
        /// </summary>
        private Thread DrawMiddleLayerThread;

        /// <summary>
        /// 内容信息
        /// </summary>
        private ViewInfo ContextViewInfo = new ViewInfo();
        /// <summary>
        /// 图片编辑器
        /// </summary>
        private PictureEditorComponents PictureEditor = new PictureEditorComponents();
        /// <summary>
        /// Style列表
        /// </summary>
        private List<StyleInfo> StyleInfos
        {
            get {
                return ContextViewInfo.StyleInfos;
            }
            set {
                if (ContextViewInfo.StyleInfos.Equals(value))
                    return;
                ContextViewInfo.StyleInfos = value;
            }
        }
        /// <summary>
        /// 段落设置列表
        /// </summary>
        private List<ParagraphInfo> ParagraphInfos
        {
            get {
                return ContextViewInfo.ParagraphInfos;
            }
            set {
                if (ContextViewInfo.ParagraphInfos.Equals(value))
                    return;
                ContextViewInfo.ParagraphInfos= value;
            }
        }
        /// <summary>
        /// 行信息
        /// </summary>
        private List<LineInfo> LineInfos
        {
            get {
                return ContextViewInfo.LineInfos;
            }
            set {
                if (ContextViewInfo.LineInfos.Equals(value))
                    return;
                ContextViewInfo.LineInfos = value;
            }
        }
        /// <summary>
        /// 所有元素
        /// </summary>
        private List<ViewItem> ContextItems
        {
            get
            {
                return ContextViewInfo.ContextItems;
            }
            set
            {
                if (ContextViewInfo.ContextItems.Equals(value))
                    return;
                ContextViewInfo.ContextItems = value;
            }
        }
        /// <summary>
        /// 绘制锁
        /// </summary>
        private readonly object IsDraw = new object();
        /// <summary>
        /// 鼠标按下时所处的itemIndex
        /// </summary>
        private long MouseStartIndex = 0;

        /// <summary>
        /// 获得焦点后第一次按键，用于处理Tab键的逻辑
        /// </summary>
        private bool KeyPressForAfterFocus = false;
        /// <summary>
        /// IME输入处理程序
        /// </summary>
        private ImeComponent IME;

        private int _curLine = 0;
        /// <summary>
        /// 当前行
        /// </summary>
        public int CurLine
        {
            get
            {
                return _curLine + 1;
            }
            private set
            {
                if (_curLine == value)
                    return;
                _curLine = value;
            }
        }
        /// <summary>
        /// 重画文本内容锁
        /// </summary>
        private object IsReDrawContextLock = new object();
      
        /// <summary>
        /// Shift键是否按下
        /// </summary>
        private bool IsShiftDown = false;
        /// <summary>
        /// 是否完成了初始化
        /// </summary>
        private bool IsInitialization = false;
        /// <summary>
        /// 是否获得了焦点
        /// </summary>
        private bool IsFocus = false;

        /// <summary>
        /// 内容Bitmap
        /// </summary>
        private Bitmap MiddleLayerBitmap = null;
        /// <summary>
        /// 内容Graphics
        /// </summary>
        private Graphics MiddleLayerGraphics = null;

        /// <summary>
        /// 光标Bitmap
        /// </summary>
        private Bitmap CursorBitmap = new Bitmap(10, 50);
        /// <summary>
        /// 光标Graphics
        /// </summary>
        private Graphics CursorGraphics = null;

        /// <summary>
        /// 合成Bitmap
        /// </summary>
        private Bitmap BlendBitmap = null;
        /// <summary>
        /// 合成Graphics
        /// </summary>
        private Graphics BlendGraphics = null;

        /// <summary>
        /// 光标位置
        /// </summary>
        internal Action<CursorPointInfo> CursorPointChange;
        private CursorPointInfo LaseCursorPoint = CursorPointInfo.Empty;
        /// <summary>
        /// 根据当前光标获取元素
        /// </summary>
        /// <returns></returns>
        private ViewItem GetCurItem(long? index = null)
        {
            if (index == null)
                index = CursorIndex;
            if (index == 0)
                return null;
            return ContextItems[(int)index - 1];
        }

        /// <summary>
        /// 根据元素获取光标位置
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="area">鼠标在Item的哪个区域</param>
        /// <returns></returns>
        private long ItemToCursor(ViewItem item, MouseInItem area)
        {
            if (area == MouseInItem.After)
                return ContextItems.IndexOf(item) + 1;
            else
                return ContextItems.IndexOf(item);
        }

        /// <summary>
        /// 根据StyleNo获取StyleInfo
        /// </summary>
        /// <param name="styleNo"></param>
        /// <returns></returns>
        private StyleInfo GetStyleInfo(int styleNo)
        {
            var style = StyleInfos.SingleOrDefault(o => o.StyleNo == styleNo);
            if (style == null)
                style = StyleInfos[0];
            return style;
        }

        /// <summary>
        /// 获取一个新的StyleNo
        /// </summary>
        /// <returns></returns>
        private int GetNewStyleNo()
        {
            var tmp = StyleInfos.Max(o => o.StyleNo);
            return tmp + 1;
        }
        #endregion

        #region WinAPI
        /// <summary>
        /// 获取按键状态
        /// </summary>
        /// <param name="nVirtKey"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "GetKeyState")]
        private static extern int GetKeyState(int nVirtKey);

        /// <summary>
        /// 消息发送API
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        #region WM
        //https://www.cnblogs.com/gaara-zhang/p/10706224.html
        /// <summary>
        /// 重画
        /// </summary>
        const int WM_PAINT = 0x000F;
        /// <summary>
        /// 获得焦点后
        /// </summary>
        const int WM_SETFOCUS = 0x0007;
        /// <summary>
        /// 失去焦点
        /// </summary>
        const int WM_KILLFOCUS = 0x0008;
        /// <summary>
        /// 当窗口背景必须被擦除时（例在窗口改变大小时）
        /// </summary>
        const int WM_ERASEBKGND = 0x0014;
        /// <summary>
        /// 当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给当前窗口
        /// </summary>
        const int WM_MOUSEACTIVATE = 0x0021;
        /// <summary>
        /// 此消息发送给窗口当它将要改变大小或位置
        /// </summary>
        const int WM_GETMINMAXINFO = 0x0024;
        /// <summary>
        /// 当用户某个窗口中点击了一下右键就发送此消息给这个窗口
        /// </summary>
        const int WM_CONTEXTMENU = 0x007B;
        /// <summary>
        /// 移动鼠标，按住或释放鼠标时发生
        /// </summary>
        const int WM_NCHITTEST = 0x0084;
        /// <summary>
        /// 按下一个键
        /// </summary>
        const int WM_KEYDOWN = 0x0100;
        /// <summary>
        /// 释放一个键
        /// </summary>
        const int WM_KEYUP = 0x0101;
        /// <summary>
        /// 按下某键，并已发出WM_KEYDOWN， WM_KEYUP消息
        /// </summary>
        const int WM_CHAR = 0x0102;
        /// <summary>
        /// 移动鼠标
        /// </summary>
        const int WM_MOUSEMOVE = 0x0200;
        /// <summary>
        /// 按下鼠标左键
        /// </summary>
        const int WM_LBUTTONDOWN = 0x0201;
        /// <summary>
        /// 释放鼠标左键
        /// </summary>
        const int WM_LBUTTONUP = 0x0202;
        /// <summary>
        /// 双击鼠标左键
        /// </summary>
        const int WM_LBUTTONDBLCLK = 0x0203;
        /// <summary>
        /// 按下鼠标右键
        /// </summary>
        const int WM_RBUTTONDOWN = 0x0204;
        /// <summary>
        /// 释放鼠标右键
        /// </summary>
        const int WM_RBUTTONUP = 0x0205;
        /// <summary>
        /// 双击鼠标右键
        /// </summary>
        const int WM_RBUTTONDBLCLK = 0x0206;
        /// <summary>
        /// 按下鼠标中键
        /// </summary>
        const int WM_MBUTTONDOWN = 0x0207;
        /// <summary>
        /// 释放鼠标中键
        /// </summary>
        const int WM_MBUTTONUP = 0x0208;
        /// <summary>
        /// 双击鼠标中键
        /// </summary>
        const int WM_MBUTTONDBLCLK = 0x0209;
        /// <summary>
        /// 当鼠标轮子转动时发送此消息个当前有焦点的控件
        /// </summary>
        const int WM_MOUSEWHEEL = 0x020A;
        /// <summary>
        /// 大小改变后
        /// </summary>
        const int WM_SIZE = 0x0005;
        /// <summary>
        /// 当用户正在调整窗口大小时发送此消息给窗口;通过此消息应用程序可以监视窗口大小和位置也可以修改他们
        /// </summary>
        const int WM_SIZING = 0x0214;
        /// <summary>
        /// 退出改变大小(只对窗体有效)
        /// </summary>
        const int WM_EXITSIZEMOVE = 0x0232;
        /// <summary>
        /// 进入改变大小(只对窗体有效)
        /// </summary>
        const int WM_ENTERSIZEMOVE = 0x0231;
        /// <summary>
        /// SIZE_MAXIMIZED
        /// </summary>
        const int SIZE_MAXIMIZED = 0x02;
        /// <summary>
        /// SIZE_RESTORED
        /// </summary>
        const int SIZE_RESTORED = 0x00;
        /// <summary>
        /// WM_CUT
        /// </summary>
        const int WM_CUT = 0x0300;
        /// <summary>
        /// WM_COPY
        /// </summary>
        const int WM_COPY = 0x0301;
        /// <summary>
        /// WM_PASTE
        /// </summary>
        const int WM_PASTE = 0x0302;
        /// <summary>
        /// WM_CLEAR
        /// </summary>
        const int WM_CLEAR = 0x0303;
        /// <summary>
        /// WM_UNDO
        /// </summary>
        const int WM_UNDO = 0x0304;
        #endregion

        #region VK
        /// <summary>
        /// Enter
        /// </summary>
        const int VK_RETURN = 13;
        /// <summary>
        /// Tab
        /// </summary>
        const int VK_TAB = 9;
        /// <summary>
        /// Backspace
        /// </summary>
        const int VK_BACK = 8;
        /// <summary>
        /// Delete
        /// </summary>
        const int VK_DELETE = 46;
        /// <summary>
        /// Clear
        /// </summary>
        const int VK_CLEAR = 12;
        /// <summary>
        /// Shift
        /// </summary>
        const int VK_SHIFT = 16;
        /// <summary>
        /// Ctrl
        /// </summary>
        const int VK_CONTROL = 17;
        /// <summary>
        /// Alt
        /// </summary>
        const int VK_ALT = 18;
        /// <summary>
        /// Esc
        /// </summary>
        const int VK_ESCAPE = 27;
        /// <summary>
        /// Space
        /// </summary>
        const int VK_SPACE = 32;
        /// <summary>
        /// Page Up
        /// </summary>
        const int VK_PRIOR = 33;
        /// <summary>
        /// Page Down
        /// </summary>
        const int VK_NEXT = 34;
        /// <summary>
        /// End
        /// </summary>
        const int VK_END = 35;
        /// <summary>
        /// Home
        /// </summary>
        const int VK_HOME = 36;
        /// <summary>
        /// Left Arrow
        /// </summary>
        const int VK_LEFT = 37;
        /// <summary>
        /// Up Arrow
        /// </summary>
        const int VK_UP = 38;
        /// <summary>
        /// Right Arrow
        /// </summary>
        const int VK_RIGHT = 39;
        /// <summary>
        /// Down Arrow
        /// </summary>
        const int VK_DOWN = 40;
        /// <summary>
        /// F1
        /// </summary>
        const int VK_F1 = 112;
        /// <summary>
        /// F2
        /// </summary>
        const int VK_F2 = 113;
        /// <summary>
        /// F3
        /// </summary>
        const int VK_F3 = 114;
        /// <summary>
        /// F4
        /// </summary>
        const int VK_F4 = 115;
        /// <summary>
        /// F5
        /// </summary>
        const int VK_F5 = 116;
        /// <summary>
        /// F6
        /// </summary>
        const int VK_F6 = 117;
        /// <summary>
        /// F7
        /// </summary>
        const int VK_F7 = 118;
        /// <summary>
        /// F8
        /// </summary>
        const int VK_F8 = 119;
        /// <summary>
        /// F9
        /// </summary>
        const int VK_F9 = 120;
        /// <summary>
        /// F10
        /// </summary>
        const int VK_F10 = 121;
        /// <summary>
        /// F11
        /// </summary>
        const int VK_F11 = 122;
        /// <summary>
        /// F12
        /// </summary>
        const int VK_F12 = 123;


        #endregion

        #region MK
        /// <summary>
        /// 鼠标移动时Shift键已经按下
        /// </summary>
        const int MK_SHIFT = 0x0004;
        /// <summary>
        /// 鼠标移动时Ctrl键已经按下
        /// </summary>
        const int MK_CONTROL = 0x0008;
        /// <summary>
        /// 鼠标移动时左键已经按下
        /// </summary>
        const int MK_LBUTTON = 0x0001;
        /// <summary>
        /// 鼠标移动时中键已经按下
        /// </summary>
        const int MK_MBUTTON = 0x0010;
        /// <summary>
        /// 鼠标移动时右键已经按下
        /// </summary>
        const int MK_RBUTTON = 0x0002;
        #endregion

        #endregion

        /// <summary>
        /// 添加Undo(最多缓存10条记录)
        /// </summary>
        /// <param name="cmd">命令</param>
        internal void AddUndo(ICommand cmd)
        {
            if (UndoStack.Count > 9)
            {
                var newUndoStack = UndoStack.Reverse().ToList();
                newUndoStack.RemoveAt(0);
                UndoStack.Clear();
                foreach (var c in newUndoStack)
                    UndoStack.Push(c);
            }
            UndoStack.Push(cmd);
        }

        /// <summary>
        /// 添加Redo(最多缓存10条记录)
        /// </summary>
        /// <param name="cmd">命令</param>
        internal void AddRedo(ICommand cmd)
        {
            if (RedoStack.Count > 9)
            {
                var newRedoStack = RedoStack.Reverse().ToList();
                newRedoStack.RemoveAt(0);
                RedoStack.Clear();
                foreach (var c in newRedoStack)
                    RedoStack.Push(c);
            }
            RedoStack.Push(cmd);
        }

        /// <summary>
        /// Undo/Redo操作
        /// </summary>
        /// <param name="viewInfo">View信息</param>
        /// <param name="cursorIndex">鼠标所在位置Index</param>
        internal void ExecuteCommand(ViewInfo viewInfo, long cursorIndex)
        {
            lock (IsDraw)
            {
                ContextViewInfo = viewInfo.Clone();
                CursorIndex = cursorIndex;
            }
            MeasureItems(0);
            //CallDrawContext();
        }

        /// <summary>
        /// 是否在IDE设计模式
        /// </summary>
        /// <returns></returns>
        private bool IsDesignMode()
        {
            bool returnFlag = false;

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
#endif

            return returnFlag;
        }

        /// <summary>
        /// 初始化信息
        /// </summary>
        private void Init()
        {
            CursorGraphics = Graphics.FromImage(CursorBitmap);
            Pen p = new Pen(new SolidBrush(Color.Black));
            p.Width = 3;
            CursorGraphics.DrawLine(p, 0, 0, 0, 500);
            p.Dispose();

            StyleInfos.Add(StyleInfo.Default);
            ParagraphInfos.Add(ParagraphInfo.Default);
            LineInfos.Add(new LineInfo(Padding.Left, Padding.Top,0, StyleInfo.Default.StyleFont.Height, 0));
            LineInfos[0].MeasureWidth(ContextViewInfo);


            PictureEditor.ChangeSize +=(option) =>
            {
                var oldView = ContextViewInfo.Clone();
                var oldCursorIndex = CursorIndex;
                ContextItems[option.ItemNo].DrawSize = option.NewSize;
                MeasureItems(option.ItemNo);
                var newRect = ContextItems[option.ItemNo].DrawRectangle;
                option.ItemRectangle = newRect;
                var newView = ContextViewInfo.Clone();
                ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
                AddUndo(cmd);
            };
            this.SizeChanged += (s, e) =>
            {
                ContextViewInfo.Layout.PageSize = this.Size;
                foreach (var l in LineInfos)
                    l.MeasureWidth(ContextViewInfo);
                MeasureItems(0);
            };
                
        }


        /// <summary>
        /// 合成UI的线程方法
        /// </summary>
        private void UIThreadMethod()
        {
            bool isShowBlack = false;
            DateTime lastTime = DateTime.Now;
            do
            {
                Thread.Sleep(1);
                TimeSpan span = DateTime.Now - lastTime;
                if (span.TotalMilliseconds >= 600)
                {
                    isShowBlack = !isShowBlack;
                    lastTime = DateTime.Now;
                }
                //isShowBlack = true;
                try
                {
                    ProcessingBlend(isShowBlack);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"RichView中Blend线程报错:{e.Message}=>{e.StackTrace}");
                }


            } while (!CancellationToken);
        }

        /// <summary>
        /// 计算光标位置
        /// </summary>
        private void CalculateCursorPoint()
        {
            try
            {
                var curInfo = GetCursorPoint();
                if (LaseCursorPoint != curInfo)
                {
                    LaseCursorPoint = curInfo;
                    CursorPointChange?.Invoke(LaseCursorPoint);
                }
            }
            catch { }
        }

        /// <summary>
        /// 处理windows消息
        /// </summary>
        /// <param name="m">消息</param>
        protected override void WndProc(ref Message m)
        {
            if (IsDesignMode())
            {
                base.WndProc(ref m);
                return;
            }

            float mouseX = 0;
            float mouseY = 0;
            switch (m.Msg)
            {
                case WM_CUT:
                    Debug.WriteLine("WM_CUT");
                    var cutStr = SelectedString();
                    if (!string.IsNullOrEmpty(cutStr))
                    {
                        BackspaceItem(true);
                        Clipboard.SetDataObject(cutStr);
                    }
                    break;
                case WM_COPY:
                    Debug.WriteLine("WM_COPY");
                    var copyStr = SelectedString();
                    if (!string.IsNullOrEmpty(copyStr))
                        Clipboard.SetDataObject(copyStr);
                    break;
                case WM_PASTE:
                    Debug.WriteLine("WM_PASTE");
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        //如果剪贴板中的数据是文本格式 
                        var psateStr = (string)iData.GetData(DataFormats.Text);//检索与指定格式相关联的数据 
                        ProcessingIMEInput(psateStr, ControlKey.None);
                    }

                    break;
                case WM_CLEAR:
                    Debug.WriteLine("WM_CLEAR");
                    break;
                case WM_UNDO:
                    Debug.WriteLine("WM_UNDO");
                    Undo();
                    Clipboard.Clear();
                    break;
                case WM_SETFOCUS:
                    Debug.WriteLine("获得焦点");
                    IsFocus = true;
                    KeyPressForAfterFocus = true;
                    break;
                case WM_KILLFOCUS:
                    Debug.WriteLine("失去焦点");
                    IsFocus = false;
                    break;
                case WM_KEYUP:
                    ProcessingKeyUp((int)m.WParam);
                    KeyPressForAfterFocus = false;
                    break;
                case WM_KEYDOWN:
                    ProcessingKeyDown((int)m.WParam);
                    break;
                case WM_SIZE:
                    ChangeGrapicsSize(LOWORD(m.LParam), HIWORD(m.LParam));
                    break;
                case WM_LBUTTONDOWN:
                    if (ContextItems.Count == 0)
                        break;
                    ClearSelect();
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);
                    if (PictureEditor.MouseInPictureEditor(mouseX, mouseY))
                    {
                        PictureEditor.OnLButtonDown(mouseX, mouseY);
                    }
                    else
                    {
                        var t = GetMouseInItem(mouseX, mouseY);
                        if (t != null)
                        {
                            CursorIndex = ItemToCursor(t.InItem, t.Area);
                            MouseStartIndex = CursorIndex;
                            ClearSelect();
                            CalculateCursorPoint();
                            CallDrawContext();
                            if (t.InItem.ItemType == ItemType.Image)
                            {
                                PictureEditor.Bind(t);
                                PictureEditor.OnLButtonDown(mouseX, mouseY);
                            }
                            else
                                PictureEditor.UnBind();
                        }
                        else
                            CalculateCursorPoint();
                    }
                    
                    break;
                case WM_MOUSEMOVE:
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);
                    if (PictureEditor.EditorMouseDown || PictureEditor.MouseInPictureEditor(mouseX, mouseY))
                    {
                        if ((int)m.WParam == MK_LBUTTON)
                            PictureEditor.OnLButtonMove(mouseX, mouseY);
                        break;
                    }
                    var endItem = GetMouseInItem(mouseX, mouseY);
                    if (endItem != null)
                    {
                        if ((int)m.WParam == MK_LBUTTON)
                        {
                            var endIndex = (int)ItemToCursor(endItem.InItem, endItem.Area);
                            MouseMoveItem((int)MouseStartIndex, (int)endIndex);
                            CursorIndex = endIndex;
                        }
                        
                    }
                    break;
                case WM_LBUTTONUP:
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);
                    if (PictureEditor.MouseInPictureEditor(mouseX, mouseY))
                        PictureEditor.OnLButtonUp(mouseX, mouseY);
                    break;
                case WM_LBUTTONDBLCLK:
                    //一般的情况下，如果没有指定CS_DBLCLKS，在窗口的消息循环里将不会得到WM_LBUTTONDBLCLK消息。
                    //如果是API创建的窗体需要在风格中设置CS_DBLCLKS
                    Debug.WriteLine($"{DateTime.Now}=>WM_LBUTTONDBLCLK");

                    break;
            }
            if (IsInitialization)
            {
                IME.ImmOperation(m); //输入法
            }
            base.WndProc(ref m);

        }

        /// <summary>
        /// 处理按下方向键和Tab键焦点转移的问题
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Debug.WriteLine($"IsFocus:{IsFocus}");
            if (IsFocus && msg.Msg == WM_KEYDOWN)
            {
                switch ((int)msg.WParam)
                {
                    case VK_TAB:
                    case VK_LEFT:
                    case VK_UP:
                    case VK_RIGHT:
                    case VK_DOWN:
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 获取低位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        private int LOWORD(IntPtr LParam)
        {
            return LParam.ToInt32() & 0x0000FFFF;
        }

        /// <summary>
        /// 获取低位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        private int LOWORD(int LParam)
        {
            return LParam & 0x0000FFFF;
        }

        /// <summary>
        /// 获取高位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        private int HIWORD(IntPtr LParam)
        {
            return (LParam.ToInt32()) >> 16;
        }

        /// <summary>
        /// 获取高位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        private int HIWORD(int LParam)
        {
            return (LParam) >> 16;
        }

        /// <summary>
        /// 改变画布大小
        /// </summary>
        private void ChangeGrapicsSize(int width, int height)
        {

            if (this.Width == 0 || this.Height == 0)
                return;
            lock (IsDraw)
            {
               
                //中间层
                MiddleLayerBitmap?.Dispose();
                MiddleLayerGraphics?.Dispose();

                //合成层
                BlendBitmap?.Dispose();
                BlendGraphics?.Dispose();
                //此实例画布
                Graphics?.Dispose();

                Graphics = this.CreateGraphics();
                MiddleLayerBitmap = new Bitmap(width, height);
                MiddleLayerGraphics = Graphics.FromImage(MiddleLayerBitmap);
                MiddleLayerGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                BlendBitmap = new Bitmap(MiddleLayerBitmap.Width, MiddleLayerBitmap.Height);
                BlendGraphics = Graphics.FromImage(BlendBitmap);
                BlendGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            }
            MeasureItems(0);
        }

        /// <summary>
        /// 获取鼠标所在的item
        /// </summary>
        private HoverItem GetMouseInItem(float x, float y)
        {
            if (ContextItems.Count == 0)
                return null;
            var lineNumber = GetMouseInLine(x, y);
            var mouseInItemType = MouseInItem.No;
            var inItem = ContextItems.Where(o => o.LineNo == lineNumber && o.PointInItem(x, y) != MouseInItem.No).Take(1).SingleOrDefault();
            if (inItem == null)
            {
                //点击没有在item上的一律处理为鼠标所在行最后一个元素之后
                inItem = ContextItems.Where(o => o.LineNo == lineNumber).LastOrDefault();
                if (inItem == null)
                    inItem = ContextItems.LastOrDefault();
                mouseInItemType = MouseInItem.After;
            }
            else
            {
                mouseInItemType = inItem.PointInItem(x, y);
            }
            return new HoverItem()
            {
                ItemNo = ContextItems.IndexOf(inItem),
                InItem = inItem,
                ItemRectangle = inItem.DrawRectangle,
                Area = mouseInItemType 
            };
            //Debug.WriteLine($"鼠标指向:{inItem.Text}=>{tmp.ToString()}");
        }

        /// <summary>
        /// 检查当前鼠标在哪一行
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GetMouseInLine(float x, float y)
        {
            
            if (LineInfos == null || LineInfos.Count == 0)
                return 1;
            int curLine = LineInfos.Count;
            var cList = LineInfos.ToList();
            cList.Reverse();
            foreach (var l in cList)
            {
                curLine--;
                if (y >= l.Top)
                {
                    return curLine;
                }

            }
            return curLine;
        }

        /// <summary>
        /// 测量元素,确定绘制位置
        /// </summary>
        /// <param name="index">开始的索引</param>
        private void MeasureItems(int index = 0)
        {

            try
            {
                if (index == -1)
                {
                    CallDrawContext();
                    return;
                }
                if (ContextItems.Count == 0)
                {
                    CallDrawContext();
                    return;
                }
                if (MiddleLayerGraphics == null)
                    return;
                lock (IsDraw)
                {
                   
                    if (index > ContextItems.Count - 1)
                        index = ContextItems.Count - 1;
                    Stopwatch sw = new Stopwatch();//Stopwatch提供一组方法和属性，可用于准确地测量运行时间
                   
                    for (int i = index; i < ContextItems.Count; i++)
                    {
                        sw.Start();
                        //使用新的测量方法
                        ContextItems[i].Measure(MiddleLayerGraphics, ContextViewInfo);
                        sw.Stop();
                        TimeSpan dt = sw.Elapsed;//获取当前实例测量得出的总运行时间
                        sw.Reset();
                        Debug.WriteLine($"[{i}]程序耗时:'{ dt.TotalMilliseconds}'毫秒");
                    }
                    
                    List<int> DelLineIndexs = new List<int>();
                    foreach (var l in LineInfos)
                    {
                        float maxHeiht = 0;
                        var lineitems = ContextItems.Where(o => o.LineNo == l.Number);
                        if (lineitems != null && lineitems.Count() > 0)
                            maxHeiht = lineitems.Max(o => o.DrawSize.Height);
                        else
                            maxHeiht = TextItem.Enter.DrawSize.Height;
                        l.Height = Convert.ToInt32(Math.Ceiling(maxHeiht));
                        if (l.Number != 0)
                        {
                            l.MeasureTop(ContextViewInfo);
                        }
                        var lineItems = ContextItems.Where(o => o.LineNo == l.Number).ToList();
                        if (lineItems != null && lineItems.Count > 0)
                        {
                            foreach (var item in lineitems)
                                item.LineHeightChange(l);
                        }
                        else
                            DelLineIndexs.Add(LineInfos.IndexOf(l));
                    }
                    //if (DelLineIndexs.Count > 0)
                    //{
                    //    DelLineIndexs.Reverse();
                    //    foreach (var i in DelLineIndexs)
                    //        LineInfos.RemoveAt(i);
                    //}
                }
                
                CallDrawContext();

            }
            catch (Exception e)
            {
                 throw e;
            }
        }

        /// <summary>
        /// 处理IME输入字符串
        /// </summary>
        /// <param name="text">热输入的字符串</param>
        /// <param name="key">控制符</param>
        private void ProcessingIMEInput(string text, ControlKey key = ControlKey.None)
        {
            if (PictureEditor.IsBind)
                return;
            var oldView = ContextViewInfo.Clone();
            var oldCursorIndex = CursorIndex;

            List<ViewItem> newItem = new List<ViewItem>();
            if (key == ControlKey.None)
            {
                var l = text.ToCharArray();
                foreach (var s in l)
                {
                    if (s.ToString() == " ")
                        newItem.Add(TextItem.Space);
                    else
                        newItem.Add(new TextItem(s.ToString(), 0));
                }
            }
            else
            {
                switch (key)
                {
                    case ControlKey.Enter:
                        newItem.Add(TextItem.Enter);
                        break;
                    case ControlKey.Tab:
                        newItem.Add(TextItem.Tab);
                        break;
                    default:
                        return;
                }

            }
            var sl = ContextItems.Where(o => o.IsSelect).ToList();
            if (sl != null && sl.Count != 0)
            {
                CursorIndex = ContextItems.IndexOf(sl[0]);
                foreach (var t in sl)
                    ContextItems.Remove(t);
                MeasureItems((int)CursorIndex);
            }
            if (CursorIndex == ContextItems.Count)
            {
                ContextItems.AddRange(newItem);
                MeasureItems((int)CursorIndex);
                //Text += text;
                CursorIndex = ContextItems.Count;
            }
            else
            {
                ContextItems.InsertRange((int)CursorIndex, newItem);
                CursorIndex += newItem.Count;
                MeasureItems(0);
            }


            var newView = ContextViewInfo.Clone();
            ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
            AddUndo(cmd);

            //OnKeyPress?.Invoke(text, false, ControlKey.None);

        }

        /// <summary>
        /// 处理键盘消息
        /// </summary>
        private void ProcessingKeyUp(int keyUp_WParam)
        {
            if (PictureEditor.IsBind && keyUp_WParam != VK_TAB)
                return;
            switch (keyUp_WParam)
            {
                case VK_SHIFT:
                    IsShiftDown = false;
                    break;
                case VK_RETURN:
                    Debug.WriteLine("按下[Enter]");
                    ProcessingIMEInput(string.Empty, ControlKey.Enter);
                    break;
                case VK_TAB:
                    if (!KeyPressForAfterFocus)
                    {
                        Debug.WriteLine($"按下[Tab],当前焦点:{IsFocus}");
                        ProcessingIMEInput(string.Empty, ControlKey.Tab);
                    }
                    break;
                case VK_LEFT:
                    Debug.WriteLine("按下[←]");
                    if (IsShiftDown)
                        MoveLast();
                    else
                    {
                        if (TextSelected)
                        {
                            ClearSelect();
                            CallDrawContext();
                        }
                        else
                            MoveLast();
                    }
                    break;
                case VK_UP:
                    Debug.WriteLine("按下[↑]");
                    if (IsShiftDown)
                        MoveLastLine();
                    else
                    {
                        if (TextSelected)
                        {
                            ClearSelect();
                            CallDrawContext();
                        }
                        else
                            MoveLastLine();
                    }
                    break;
                case VK_RIGHT:
                    Debug.WriteLine("按下[→]");
                    if (IsShiftDown)
                        MoveNext();
                    else
                    {
                        if (TextSelected)
                        {
                            ClearSelect();
                            CallDrawContext();
                        }
                        else
                            MoveNext();
                    }
                    break;
                case VK_DOWN:
                    Debug.WriteLine("按下[↓]");
                    if (IsShiftDown)
                        MoveNextLine();
                    else
                    {
                        if (TextSelected)
                        {
                            ClearSelect();
                            CallDrawContext();
                        }
                        else
                            MoveNextLine();
                    }
                    break;

            }

        }

        /// <summary>
        /// 处理键盘消息
        /// </summary>
        private void ProcessingKeyDown(int keyUp_WParam)
        {
            //bool isControlKey = true;
            switch (keyUp_WParam)
            {
                case VK_SHIFT:
                    IsShiftDown = true;
                    break;
                case VK_BACK:
                    Debug.WriteLine("按下[Backspace]");
                    BackspaceItem();
                    break;
                case VK_DELETE:
                    Debug.WriteLine("按下[Delete]");
                    DeleteItem();
                    break;
                case VK_PRIOR:
                    Debug.WriteLine("按下[PageUp]");
                    break;
                case VK_NEXT:
                    Debug.WriteLine("按下[PageDown]");
                    break;
                case VK_HOME:
                    Debug.WriteLine("按下[Home]");
                    break;
                case VK_END:
                    Debug.WriteLine("按下[End]");
                    break;
                case (int)Keys.A:
                    if (IsCmdKeyDown(VK_CONTROL) && !IsCmdKeyDown(VK_ALT))
                    {
                        Debug.WriteLine("Ctl+A");
                        SetSelectAll();
                    }
                    break;
                case (int)Keys.Z:
                    if (IsCmdKeyDown(VK_CONTROL) && !IsCmdKeyDown(VK_ALT))
                    {
                        Debug.WriteLine("Ctl+Z");
                        SendMessage(this.Handle, WM_UNDO, 0, 0);
                    }
                    break;
                case (int)Keys.X:
                    if (IsCmdKeyDown(VK_CONTROL) && !IsCmdKeyDown(VK_ALT))
                    {
                        Debug.WriteLine("Ctl+X");
                        SendMessage(this.Handle, WM_CUT, 0, 0);
                    }
                    break;
                case (int)Keys.C:
                    if (IsCmdKeyDown(VK_CONTROL) && !IsCmdKeyDown(VK_ALT))
                    {
                        Debug.WriteLine("Ctl+C");
                        SendMessage(this.Handle, WM_COPY, 0, 0);
                    }
                    break;
                case (int)Keys.V:
                    if (IsCmdKeyDown(VK_CONTROL) && !IsCmdKeyDown(VK_ALT))
                    {
                        Debug.WriteLine("Ctl+V");
                        SendMessage(this.Handle, WM_PASTE, 0, 0);
                    }
                    break;

                default:
                    //isControlKey = false;
                    break;
            }

            //if (isControlKey)
            //{
            //    OnKeyPress?.Invoke(string.Empty, true, (ControlKey)keyUp_WParam);
            //}
        }

        /// <summary>
        /// 检查一个命令按键是否按下
        /// </summary>
        /// <param name="vk_code">虚拟键码</param>
        /// <returns></returns>
        private bool IsCmdKeyDown(int vk_code)
        {
            if (LOWORD(GetKeyState(vk_code)) > 1)
                return true;
            return false;
        }

        /// <summary>
        /// 向后删除元素
        /// </summary>
        private void DeleteItem()
        {
            if (ContextItems.Count > 0)
            {
                var oldView = ContextViewInfo.Clone();
                var oldCursorIndex = CursorIndex;

                var sl = ContextItems.Where(o => o.IsSelect).ToList();
                if (sl != null && sl.Count != 0)
                {
                    CursorIndex = ContextItems.IndexOf(sl[0]);
                    foreach (var t in sl)
                        ContextItems.Remove(t);
                    MeasureItems((int)CursorIndex);

                    var newView = ContextViewInfo.Clone();
                    ViewCommand cmd = new ViewCommand(this, oldView,newView, oldCursorIndex, CursorIndex);
                    AddUndo(cmd);
                    return;
                }
                if (CursorIndex == ContextItems.Count)
                    return;
                if (CursorIndex <= ContextItems.Count)
                {
                    if (CursorIndex >= 0)
                    {
                        ContextItems.RemoveAt((int)CursorIndex);
                        MeasureItems((int)CursorIndex);

                        var newView = ContextViewInfo.Clone();
                        ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
                        AddUndo(cmd);
                    }
                }

            }
        }

        /// <summary>
        /// 向前删除元素
        /// </summary>
        /// <param name="isCutCommand">是否为Ctrl+X命令调用</param>
        private void BackspaceItem(bool isCutCommand = false)
        {
            if (ContextItems.Count > 0)
            {
                var oldView = ContextViewInfo.Clone();
                var oldCursorIndex = CursorIndex;

                var sl = ContextItems.Where(o => o.IsSelect).ToList();
                if (sl != null && sl.Count != 0)
                {
                    CursorIndex = ContextItems.IndexOf(sl[0]);
                    foreach (var t in sl)
                        ContextItems.Remove(t);
                    MeasureItems((int)CursorIndex);

                    var newView = ContextViewInfo.Clone();
                    ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
                    AddUndo(cmd);
                    return;
                }
                if (isCutCommand)
                    return;
                if (CursorIndex == ContextItems.Count)
                {
                    ContextItems.RemoveAt((int)CursorIndex - 1);
                    MeasureItems(-1);
                    //Text = Text.Remove(Text.Length - 1, 1);
                    CursorIndex--;

                    var newView = ContextViewInfo.Clone();
                    ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
                    AddUndo(cmd);
                }
                else
                {
                    if (CursorIndex > 0)
                    {
                        ContextItems.RemoveAt((int)CursorIndex - 1);
                        MeasureItems((int)CursorIndex - 1);
                        CursorIndex--;

                        var newView = ContextViewInfo.Clone();
                        ViewCommand cmd = new ViewCommand(this, oldView, newView, oldCursorIndex, CursorIndex);
                        AddUndo(cmd);
                    }
                }

            }
        }

        /// <summary>
        /// 移动到下一行
        /// </summary>
        private void MoveNextLine()
        {
            lock (IsDraw)
            {
                if (CurLine < LineInfos.Count)
                {
                    if (ContextItems.Count == 0)
                        return;
                    long oldIndex = CursorIndex;

                    ViewItem item;
                    if (CursorIndex > 0)
                        item = ContextItems[(int)CursorIndex - 1];
                    else
                        item = ContextItems[0];
                    var t = ContextItems.Where(o => o.LineNo == item.LineNo + 1 && o.DrawRectangle.Left <= item.DrawRectangle.Right + 2).OrderByDescending(o => o.DrawRectangle.Left).Take(1).SingleOrDefault();
                    if (t == null)
                        return;
                    var newIndex = ContextItems.IndexOf(t);
                    CursorIndex = newIndex;
                    if (IsShiftDown)
                    {
                        for (int i = (int)oldIndex + 1; i <= (int)CursorIndex; i++)
                            GetCurItem(i).IsSelect = !GetCurItem(i).IsSelect;
                    }
                    else
                    {
                        ClearSelect();
                    }
                    CalculateCursorPoint();
                    CallDrawContext();
                }
            }
        }

        /// <summary>
        /// 移动到上一行
        /// </summary>
        private void MoveLastLine()
        {
            lock (IsDraw)
            {
                if (CurLine > 1)
                {
                    if (ContextItems.Count == 0)
                        return;
                    long oldIndex = CursorIndex;

                    ViewItem item;
                    if (CursorIndex > 0)
                        item = ContextItems[(int)CursorIndex - 1];
                    else
                        item = ContextItems[0];
                    var t = ContextItems.Where(o => o.LineNo == item.LineNo - 1 && o.DrawRectangle.Left <= item.DrawRectangle.Right + 2).OrderByDescending(o => o.DrawRectangle.Left).Take(1).SingleOrDefault();
                    if (t == null)
                        return;
                    var newIndex = ContextItems.IndexOf(t);
                    CursorIndex = newIndex;
                    if (IsShiftDown)
                    {
                        for (int i = (int)CursorIndex + 1; i <= (int)oldIndex; i++)
                            GetCurItem(i).IsSelect = !GetCurItem(i).IsSelect;
                        
                    }
                    else
                    {
                        ClearSelect();
                    }
                    CalculateCursorPoint();
                    CallDrawContext();
                }
            }
        }
        /// <summary>
        /// 移动到上一个元素
        /// </summary>
        private void MoveLast()
        {
            lock (IsDraw)
            {
                if (CursorIndex > 0)
                {
                    CursorIndex--;
                    if (IsShiftDown)
                    {
                        GetCurItem(CursorIndex + 1).IsSelect = !GetCurItem(CursorIndex + 1).IsSelect;
                        
                    }
                    else
                    {
                        ClearSelect();
                    }
                    CalculateCursorPoint();
                }
            }
            CallDrawContext();
        }
        /// <summary>
        /// 移动到下一个元素
        /// </summary>
        private void MoveNext()
        {
            lock (IsDraw)
            {
                if (CursorIndex < ContextItems.Count)
                {
                    CursorIndex++;
                    if (IsShiftDown)
                    {
                        GetCurItem(CursorIndex).IsSelect = !GetCurItem(CursorIndex).IsSelect;
                        
                    }
                    else
                    {
                        ClearSelect();
                    }
                    CalculateCursorPoint();
                }

            }
            CallDrawContext();
        }

        /// <summary>
        /// 鼠标滑过item
        /// </summary>
        /// <param name="startIndex">鼠标开始滑动的item</param>
        /// <param name="endIndex">鼠标结束滑动的item</param>
        private void MouseMoveItem(int startIndex, int endIndex)
        {
            if (ContextItems.Count == 0)
                return;
            if (endIndex < startIndex)
            {
                startIndex = startIndex ^ endIndex;
                endIndex = endIndex ^ startIndex;
                startIndex = startIndex ^ endIndex;
            }
            ClearSelect();

            for (int index = startIndex; index < endIndex; index++)
                ContextItems[index].IsSelect = !ContextItems[index].IsSelect;
            CalculateCursorPoint();
            CallDrawContext();
        }

        /// <summary>
        /// 清除选中状态
        /// </summary>
        private void ClearSelect()
        {
            if (ContextItems.Count == 0)
                return;
            var list = ContextItems.Where(o => o.IsSelect).ToList();
            if (list == null)
                return;
            foreach (var t in list)
                t.IsSelect = false;
            
        }

        /// <summary>
        /// 获取当前选中的文本
        /// </summary>
        /// <returns></returns>
        private string SelectedString()
        {
            var list = SelectedItem();
            if (list == null || list.Count == 0)
                return string.Empty;
            StringBuilder s = new StringBuilder();
            foreach (var t in list)
                s.Append(t.Text);
            return s.ToString();
        }

        /// <summary>
        /// 获取所有文本内容
        /// </summary>
        /// <returns></returns>
        private string AllString()
        {
            var list = ContextItems.ToList();
            if (list == null || list.Count == 0)
                return string.Empty;
            StringBuilder s = new StringBuilder();
            foreach (var t in list)
                s.Append(t.Text);
            return s.ToString();
        }

        /// <summary>
        /// 当前选中的Iten
        /// </summary>
        /// <returns></returns>
        private List<ViewItem> SelectedItem()
        {
            if (ContextItems.Count == 0)
                return null;
            var list = ContextItems.Where(o => o.IsSelect).ToList();
            return list;

        }

        /// <summary>
        /// 选中相应文本
        /// </summary>
        /// <param name="startIndex">开始位置</param>
        /// <param name="endIndex">结束位置</param>
        private void SetSelect(int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
                throw new Exception("endIndex不能小于startIndex");
            if (startIndex < 0)
                throw new Exception("startIndex不能小于0");
            if (endIndex > ContextItems.Count - 1)
                endIndex = ContextItems.Count - 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                ContextItems[i].IsSelect = true;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void SetSelectAll()
        {
            SetSelect(0, ContextItems.Count - 1);
            CursorIndex = ContextItems.Count - 1;
            CalculateCursorPoint();
            CallDrawContext();
        }

        /// <summary>
        /// 获取当前光标位置信息
        /// </summary>
        /// <returns></returns>
        private CursorPointInfo GetCursorPoint()
        {
            try
            {

                var info = new CursorPointInfo(0, 0, 10, 1);
                if (CursorIndex == 0 || ContextItems.Count == 0)
                {
                    var tSize = MiddleLayerGraphics.MeasureString("测", StyleInfos[0].StyleFont, 100, StringFormat.GenericTypographic);
                    info.Height = tSize.Height;
                    info.Left = Padding.Left;
                    info.Top = Padding.Top;
                    return info;
                }
                var curItem = ContextItems[(int)CursorIndex - 1];
                CurLine = curItem.LineNo;
                var line = LineInfos[curItem.LineNo];

                info.Left = curItem.DrawRectangle.Right + 2;
                info.Height = line.Height;
                info.Top = line.Top;
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 绘制背景层
        /// </summary>
        private void DrawBackgroundLayer(Graphics backgroundLayerGraphics,Brush selectPen, List<ViewItem> copylist)
        {
            try
            {
              
                var list = copylist.Where(o => o.IsSelect).ToList();
                if (list == null || list.Count == 0)
                    return;
                var minLine = list.Select(o => o.LineNo).Min();
                var maxLine = list.Select(o => o.LineNo).Max();
                var minLeft = list.Where(o => o.LineNo == minLine).Min(o => o.DrawRectangle.Left);
                var maxRight = list.Where(o => o.LineNo == maxLine).Max(o => o.DrawRectangle.Right);
                if (minLine == maxLine)
                {
                    var lineInfo = LineInfos[minLine];
                    backgroundLayerGraphics.FillRectangle(selectPen, new RectangleF(minLeft, lineInfo.Top, maxRight - minLeft, lineInfo.Height));
                }
                else
                {
                    for (var i = minLine; i <= maxLine; i++)
                    {
                        var lineInfo = LineInfos[i];
                        if (i == minLine)
                            backgroundLayerGraphics.FillRectangle(selectPen, new RectangleF(minLeft, lineInfo.Top, this.Width - minLeft, lineInfo.Height));
                        else if (i == maxLine)
                            backgroundLayerGraphics.FillRectangle(selectPen, new RectangleF(0, lineInfo.Top, maxRight, lineInfo.Height));
                        else
                            backgroundLayerGraphics.FillRectangle(selectPen, new RectangleF(0, lineInfo.Top, this.Width, lineInfo.Height));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 启动绘制内容的线程
        /// </summary>
        private void CallDrawContext()
        {
            lock (IsReDrawContextLock)
            {
                DrawContextQueue.Enqueue(0);
            }
           
        }

        /// <summary>
        /// 文字改变或样式改变后重新绘制内容
        /// </summary>
        private void DrawMiddleLayer()
        {
            Brush enterPointBrush = new SolidBrush(Color.Gray);
            Brush selectBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            
            do
            {
                Thread.Sleep(1);
                lock (IsReDrawContextLock)
                {
                    if (DrawContextQueue.Count <= 0)
                        continue;
                    DrawContextQueue.Clear();
                }
                try
                {
                    if (MiddleLayerGraphics == null)
                        return;

                    lock (IsDraw)
                    {
                        var DrawEnterSize = MiddleLayerGraphics.MeasureString("↵", GetStyleInfo(0).StyleFont, 800, StringFormat.GenericTypographic);
                        List<ViewItem> copylist = ContextItems.ToList();
                        MiddleLayerGraphics.Clear(Color.White);
                        DrawBackgroundLayer(MiddleLayerGraphics, selectBrush, copylist);

                        foreach (var s in copylist)
                        {
                            s.Draw(MiddleLayerGraphics,ContextViewInfo);
                        }
                        CalculateCursorPoint();
                    }
                    
                }
                catch (Exception e)
                {
                    throw e;
                }

            } while (!CancellationToken);

            enterPointBrush.Dispose();
            selectBrush.Dispose();
        }

        /// <summary>
        /// 处理合成
        /// </summary>
        /// <param name="isHighlight"></param>
        private void ProcessingBlend(bool isHighlight)
        {
            lock (IsDraw)
            {
                BlendGraphics?.Clear(this.BackColor);
                BlendGraphics?.DrawImage(MiddleLayerBitmap, 0, 0);
                

                if (IsFocus && isHighlight && !PictureEditor.IsBind)
                    BlendGraphics?.DrawImage(CursorBitmap, LaseCursorPoint.Left, LaseCursorPoint.Top, LaseCursorPoint.Width, LaseCursorPoint.Height);
                PictureEditor.Draw(BlendGraphics);
                Graphics?.DrawImage(BlendBitmap, 0, 0);

            }
            //OnDraw?.Invoke(ContextGraphics);
        }

        private void GCThreadMethod()
        {
            do
            {
                CallGC();
                Thread.Sleep(3000);
            } while (!CancellationToken);
        }
        /// <summary>
        /// 强制GC
        /// </summary>
        private void CallGC()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            CancellationToken = true;
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            
            base.Dispose(disposing);
        }

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RichView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "RichView";
            this.Size = new System.Drawing.Size(400, 200);
            this.ResumeLayout(false);

        }

        #endregion
    }

}
