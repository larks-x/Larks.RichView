namespace Larks.RichView.Components
{
    /// <summary>
    /// 输入法组件
    /// </summary>
    public class ImeComponent
    {
        #region Event
        /// <summary>
        /// 输入文本事件
        /// </summary>
        public delegate void InputTextEvent(string text);
        /// <summary>
        /// 输入文本事件
        /// </summary>
        public event InputTextEvent InputText;
        #endregion

        #region PrivateField
        private IntPtr hIMC;
        private IntPtr handle;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_CHAR = 0x0102;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTSTR = 0x0800;
        private const int GCS_COMPSTR = 0x0008;
        private const int CFS_DEFAULT = 0x0000;
        private const int CFS_RECT = 0x0001;
        private const int CFS_POINT = 0x0002;
        private const int CFS_SCREEN = 0x0004;
        private const int CFS_FORCE_POSITION = 0x0020;
        private const int CFS_CANDIDATEPOS = 0x0040;
        private const int CFS_EXCLUDE = 0x0080;
        private bool IsRichView = false;
        //CursorPointInfo CursorInfo = CursorPointInfo.Empty;
        #endregion

        #region Construction
        /// <summary>
        /// 输入法组件
        /// </summary>
        /// <param name="control"></param>
        public ImeComponent(UserControl control)
        {
            var handle = control.Handle;
            hIMC = ImmGetContext(handle);
            if (control is RichView)
            {
                IsRichView = true;
                //((DrawingManaged)control).CursorPointChange += (c) =>
                //{
                //    CursorInfo = c;
                //    //输入法位置
                //    if (IsRichView && CursorInfo != CursorPointInfo.Empty)
                //    {
                //        COMPOSITIONFORM cf = new();
                //        cf.dwStyle = CFS_POINT;
                //        cf.ptCurrentPos.X = (int)CursorInfo.Left;
                //        cf.ptCurrentPos.Y = (int)CursorInfo.Top;
                //        cf.rcArea.Bottom = 10;
                //        cf.rcArea.Top = 10;
                //        cf.rcArea.Left = 10;
                //        cf.rcArea.Right = 10;
                //        ImmSetCompositionWindow(hIMC, ref cf);
                //    }
                //};

            }
            this.handle = handle;
        }
        /// <summary>
        /// 输入法组件
        /// </summary>
        /// <param name="from"></param>
        public ImeComponent(Form from)
        {
            var handle = from.Handle;
            hIMC = ImmGetContext(handle);
            this.handle = handle;
        }

        #endregion

        #region Method
        /// <summary>
        /// 输入事件
        /// </summary>
        /// <param name="m"></param>
        public void ImmOperation(Message m)
        {
            if (m.Msg == ImeComponent.WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1)
            {
                this.ImmAssociateContext(handle);
            }
            else if (m.Msg == WM_IME_COMPOSITION)
            {
                //输入法位置
                //if (IsRichView && CursorInfo != CursorPointInfo.Empty)
                //{
                //    COMPOSITIONFORM cf = new();
                //    cf.dwStyle = CFS_POINT;
                //    cf.ptCurrentPos.X = (int)CursorInfo.Left;
                //    cf.ptCurrentPos.Y = (int)CursorInfo.Top;
                //    cf.rcArea.Bottom = 10;
                //    cf.rcArea.Top = 10;
                //    cf.rcArea.Left = 10;
                //    cf.rcArea.Right = 10;
                //    ImmSetCompositionWindow(hIMC, ref cf);
                //}
                var res = m.WParam.ToInt32();
                string text = CurrentCompStr(this.handle);
                if (!string.IsNullOrEmpty(text))
                {
                    InputText(text);
                }
            }
            else if (m.Msg == WM_CHAR)
            {
                //输入法位置
                //if (IsRichView && CursorInfo != CursorPointInfo.Empty)
                //{
                //    COMPOSITIONFORM cf = new();
                //    cf.dwStyle = CFS_POINT;
                //    cf.ptCurrentPos.X = (int)CursorInfo.Left;
                //    cf.ptCurrentPos.Y = (int)CursorInfo.Top;
                //    cf.rcArea.Bottom = 10;
                //    cf.rcArea.Top = 10;
                //    cf.rcArea.Left = 10;
                //    cf.rcArea.Right = 10;
                //    ImmSetCompositionWindow(hIMC, ref cf);
                //}
                char inputchar = (char)m.WParam;
                if (inputchar > 31 && inputchar < 127)
                {
                    InputText(inputchar.ToString());
                }
            }
        }
        /// <summary>
        /// 当前输入的字符串流
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public string CurrentCompStr(IntPtr handle)
        {
            try
            {
                int strLen = ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, null, 0);
                if (strLen > 0)
                {
                    var buffer = new byte[strLen];
                    ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, buffer, strLen);
                    return Encoding.Unicode.GetString(buffer);
                }
                else
                {
                    return string.Empty;
                }
            }
            finally
            {
                ImmReleaseContext(handle, hIMC);
            }
        }

        #endregion

        #region Win Api
        /// <summary>
        /// 建立指定输入环境与窗口之间的关联
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private IntPtr ImmAssociateContext(IntPtr hWnd)
        {
            return ImeComponent.ImmAssociateContext(hWnd, hIMC);
        }

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern int ImmCreateContext();
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmDestroyContext(int hImc);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lPBuf, int dwBufLen);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public Point ptCurrentPos;
            public RECT rcArea;
        }
        #endregion
    }
}
