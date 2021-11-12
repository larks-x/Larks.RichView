using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView
{
    public abstract class BaseHost:IDisposable
    {
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
        /// 创建插入符
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hBitmap"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth,  int nHeight);
        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        private static extern bool GetCaretPos(out Point lpPoint);
        [DllImport("user32.dll")]
        private static extern bool DestroyCaret();
        /// <summary>
        /// 获取显示插入符闪烁的毫秒数
        /// </summary>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "GetCaretBlinkTime")]
        private static extern uint GetCaretBlinkTime();

        /// <summary>
        /// 设置显示插入符闪烁的毫秒数
        /// </summary>
        /// <param name="uMSeconds">毫秒数</param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "SetCaretBlinkTime")]
        private static extern bool SetCaretBlinkTime(uint uMSeconds);

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
        /// 页面信息
        /// </summary>
        public RichViewInformation RichViewInfo = new();

        /// <summary>
        /// IME组件
        /// </summary>
        protected ImeComponent IME;

        /// <summary>
        /// 绑定控件
        /// </summary>
        protected UserControl BindUserControl;
        private IntPtr handle = IntPtr.Zero;
        /// <summary>
        /// host绑定的handle
        /// </summary>
        public IntPtr Handle {
            get => handle;
            protected set
            {
                if (handle == value)
                    return;
               
                handle = value;
                if (IME != null)
                    IME = null;
                IME = new ImeComponent(handle);
                IME.InputText += (s) =>
                {
                    ProcessingIMEInput(s);
                };
            }
        }

        /// <summary>
        /// 获得焦点后第一次按键，用于处理Tab键的逻辑
        /// </summary>
        protected bool KeyPressForAfterFocus = false;
        /// <summary>
        /// Shift键是否按下
        /// </summary>
        protected bool IsShiftDown = false;
        /// <summary>
        /// 是否完成了初始化
        /// </summary>
        protected bool IsInitialization = false;
        /// <summary>
        /// 是否获得了焦点
        /// </summary>
        protected bool IsFocus = false;
        
        /// <summary>
        /// 输入字符串或控制符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        protected abstract void InputText(string text, ControlKey key);

        /// <summary>
        /// 处理IME输入字符串
        /// </summary>
        /// <param name="text">热输入的字符串</param>
        /// <param name="key">控制符</param>
        protected void ProcessingIMEInput(string text, ControlKey key = ControlKey.None)
        {
            Debug.WriteLine($"控制键:{key.ToString()};输入:{text}");
            InputText(text, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public void WndProc(ref Message m)
        {
            float mouseX = 0;
            float mouseY = 0;
            int width = 0;
            int height = 0;
            switch (m.Msg)
            {
                case WM_CUT:
                    Debug.WriteLine("WM_CUT");
                    break;
                case WM_COPY:
                    Debug.WriteLine("WM_COPY");

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
                    Clipboard.Clear();
                    break;
                case WM_SETFOCUS:
                    Debug.WriteLine("获得焦点");
                    ShowCaret();
                    
                    Debug.WriteLine("三所毫秒数"+GetCaretBlinkTime().ToString());
                    IsFocus = true;
                    KeyPressForAfterFocus = true;
                    break;
                case WM_KILLFOCUS:
                    Debug.WriteLine("失去焦点");
                    HideCaret();
                    
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
                    width = LOWORD(m.LParam);
                    height = HIWORD(m.LParam);
                    if (RichViewInfo.ViewGraphics != null)
                        RichViewInfo.ViewGraphics = null;
                    RichViewInfo.ViewGraphics = BindUserControl.CreateGraphics();
                    break;
                case WM_LBUTTONDOWN:
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);
                    

                    break;
                case WM_MOUSEMOVE:
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);

                    break;
                case WM_LBUTTONUP:
                    mouseX = LOWORD(m.LParam);
                    mouseY = HIWORD(m.LParam);

                    break;
                case WM_LBUTTONDBLCLK:
                    //一般的情况下，如果没有指定CS_DBLCLKS，在窗口的消息循环里将不会得到WM_LBUTTONDBLCLK消息。
                    //如果是API创建的窗体需要在风格中设置CS_DBLCLKS
                    Debug.WriteLine($"{DateTime.Now}=>WM_LBUTTONDBLCLK");

                    break;
            }
            if (IsInitialization)
            {
                IME?.ImmOperation(m); //输入法
            }
        }

        /// <summary>
        /// 获取低位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        protected int LOWORD(IntPtr LParam)
        {
            return LParam.ToInt32() & 0x0000FFFF;
        }

        /// <summary>
        /// 获取低位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        protected int LOWORD(int LParam)
        {
            return LParam & 0x0000FFFF;
        }

        /// <summary>
        /// 获取高位数据
        /// </summary>
        /// <param name="LParam"></param>
        /// <returns></returns>
        protected int HIWORD(IntPtr LParam)
        {
            return (LParam.ToInt32()) >> 16;
        }

        /// <summary>
        /// 处理键盘消息
        /// </summary>
        protected void ProcessingKeyUp(int keyUp_WParam)
        {

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

                    break;
                case VK_UP:
                    Debug.WriteLine("按下[↑]");

                    break;
                case VK_RIGHT:
                    Debug.WriteLine("按下[→]");

                    break;
                case VK_DOWN:
                    Debug.WriteLine("按下[↓]");

                    break;

            }

        }

        /// <summary>
        /// 处理键盘消息
        /// </summary>
        protected void ProcessingKeyDown(int keyUp_WParam)
        {
            //bool isControlKey = true;
            switch (keyUp_WParam)
            {
                case VK_SHIFT:
                    IsShiftDown = true;
                    break;
                case VK_BACK:
                    Debug.WriteLine("按下[Backspace]");
                    break;
                case VK_DELETE:
                    Debug.WriteLine("按下[Delete]");
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
        protected bool IsCmdKeyDown(int vk_code)
        {
            if (LOWORD(GetKeyState(vk_code)) > 1)
                return true;
            return false;
        }

        /// <summary>
        /// 是否在IDE设计模式
        /// </summary>
        /// <returns></returns>
        protected bool IsDesignMode()
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
        /// 创建光标
        /// </summary>
        protected void CreateCaret()
        {
            //创建光标
            CreateCaret(Handle, IntPtr.Zero, 0, 20);
            SetCaretBlinkTime(600);
        }

        /// <summary>
        /// 显示光标
        /// </summary>
        protected void ShowCaret()
        {
            HideCaret();
            CreateCaret();
            ShowCaret(Handle);

        }

        /// <summary>
        /// 隐藏光标
        /// </summary>
        protected void HideCaret()
        {
            HideCaret(Handle);
            DestroyCaret();
        }

        /// <summary>
        /// 移动插入符到指定位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void MoveCaretPos(int x,int y)
        {
            ShowCaret();
            SetCaretPos(x, y);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            //销毁光标
            DestroyCaret();
        }
    }
}
