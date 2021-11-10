using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView
{
    /// <summary>
    /// RichView主机
    /// </summary>
    public class RichViewHost : BaseHost
    {
     
        /// <summary>
        /// 创建RichViewHost
        /// </summary>
        /// <param name="userControl">主机挂在的自定义控件</param>
        public RichViewHost(UserControl userControl)
        {
            Handle = userControl.Handle;          
            IME = new ImeComponent(userControl);
            IME.InputText += (s) =>
            {
                ProcessingIMEInput(s);
            };
            IsInitialization = true;
            
        }

        /// <summary>
        /// 输入字符串或控制符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void InputText(string text, ControlKey key)
        {
            MoveSetCaretPos(100, 100);
            //throw new NotImplementedException();
        }
    }
}
