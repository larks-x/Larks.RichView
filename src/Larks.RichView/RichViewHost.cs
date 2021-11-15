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
        /// <param name="userControl">主机挂载的自定义控件</param>
        public RichViewHost(UserControl userControl):base()
        {
            //userControl.DoubleBuffered = true;
            Handle = userControl.Handle;
            BindUserControl = userControl;
            RichViewInfo.ViewGraphics= BindUserControl.CreateGraphics();
            RichViewInfo.CreateBuffGraphics(BindUserControl.Width, BindUserControl.Height);
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
            List<IContentItem> items = new List<IContentItem>();
            if (key == ControlKey.None)
            {
                var array = text.ToCharArray();
                array.ToList().ForEach((item) =>
                {
                    if (item.ToString() == Constant.Space)
                        items.Add(TextItem.Space);
                    else
                        items.Add(new TextItem(item.ToString()));

                });
            }
            else
            {
                switch (key)
                {
                    case ControlKey.Enter:
                        items.Add(TextItem.Enter);
                        break;
                    case ControlKey.Tab:
                        items.Add(TextItem.Tab);
                        break;
                    default:
                        return;
                }
            }
            RichViewInfo.AddRangeItem(items);
            
        }
    }
}
