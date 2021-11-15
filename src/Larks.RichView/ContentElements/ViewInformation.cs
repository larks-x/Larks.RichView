using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larks.RichView.ContentElements
{
    /// <summary>
    /// RichView页面信息
    /// </summary>
    public class RichViewInformation:IDisposable
    {
        private object CallDraw =new();
        private Graphics _BuffGraphics;
        private Bitmap _BuffBitmap;
        private Graphics _ViewGraphics;

        /// <summary>
        /// 启用行绘制模式
        /// </summary>
        [JsonIgnore]
        public bool UseLineModel { get; set; } = true;
        /// <summary>
        /// View画布
        /// </summary>
        [JsonIgnore]
        public Graphics ViewGraphics {
            get => _ViewGraphics;
            set {
                if (_ViewGraphics == value)
                    return;
                lock (CallDraw)
                {
                    _ViewGraphics = value;
                    OnDraw?.Invoke(_ViewGraphics);
                }
            }
        }

        /// <summary>
        /// 缓存画布
        /// </summary>
        [JsonIgnore]
        public Graphics BuffGraphics => _BuffGraphics;

        /// <summary>
        /// 创建缓存画布
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void CreateBuffGraphics(int width,int height)
        {
            if (_BuffBitmap != null)
            {
                _BuffGraphics?.Dispose();
                _BuffBitmap = null;
            }
            _BuffBitmap = new Bitmap(width, height);
            
            _BuffGraphics = Graphics.FromImage(_BuffBitmap);
            _BuffGraphics.SmoothingMode = SmoothingMode.HighQuality;
            _BuffGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        public Action<Graphics> OnDraw;
        internal void InvokOnDraw()
        {
            lock (CallDraw)
            {
                Debug.WriteLine($"开始绘制");
                _BuffGraphics.Clear(Color.White);
                OnDraw?.Invoke(_BuffGraphics);
                Debug.WriteLine($"所有元素绘制完成");
                Debug.WriteLine($"清屏");
                //_ViewGraphics.Clear(Color.White);
                Debug.WriteLine($"翻转");
                _ViewGraphics.DrawImage(_BuffBitmap, 0, 0);
            }
            
        }

        /// <summary>
        /// 光标所在的索引
        /// </summary>
        [JsonIgnore]
        public int CursorIndex { get; internal set; } = -1;
        /// <summary>
        /// 光标所在行
        /// </summary>
        public int CursorLine { get; internal set; } = -1;
        /// <summary>
        /// 布局信息
        /// </summary>
        public ViewLayout Layout = new();
        /// <summary>
        /// 字体风格
        /// </summary>
        public List<StyleInfo> Styles = new();
        
        /// <summary>
        /// RichView的元素
        /// </summary>
        public ContainerList<IContentItem> ContentItems = new();

        /// <summary>
        /// RichView的行
        /// </summary>
        public ContainerList<ContentLine> ContentLines = new();

        public RichViewInformation()
        {
           
            Styles.Add(StyleInfo.Default);
            ContentItems.ItemAdd += (item) =>{
                item.RichViewInfo = this;
                if (!UseLineModel)
                    item.Measure();
                else
                {
                    ContentLines[CursorLine].AddItem(item);
                    item.LineNo = CursorLine;
                    //ContentLines[CursorLine].Measure();
                    item.Measure();
                    ContentLines[CursorLine].Measure();
                }
                InvokOnDraw();
            };
            ContentItems.ItemAddRange += (items) =>
            {
                items.ToList().ForEach(item =>
                {
                    item.RichViewInfo = this;
                    if (!UseLineModel)
                        item.Measure();
                    else
                    {
                        ContentLines[CursorLine].AddItem(item);
                        item.LineNo = CursorLine;
                        //ContentLines[CursorLine].Measure();
                        item.Measure();
                        ContentLines[CursorLine].Measure();

                    }
                });
                InvokOnDraw();
            };
            ContentItems.ItemInsert += (index,item) => {
                item.RichViewInfo = this;
                if (!UseLineModel)
                    item.Measure();
                else
                {
                    ContentLines[CursorLine].AddItem(item);
                    item.LineNo = CursorLine;
                    //ContentLines[CursorLine].Measure();
                    item.Measure();
                    ContentLines[CursorLine].Measure();
                }
                InvokOnDraw();
            };
            ContentItems.ItemInsertRange += (index,items) =>
            {
                items.ToList().ForEach(item =>
                {
                    item.RichViewInfo = this;
                    if (!UseLineModel)
                        item.Measure();
                    else
                    {
                        ContentLines[CursorLine].AddItem(item);
                        item.LineNo = CursorLine;
                        //ContentLines[CursorLine].Measure();
                        item.Measure();
                        ContentLines[CursorLine].Measure();
                    }
                });
                InvokOnDraw();
            };
            ContentLines.ItemAdd += (line) =>
            {
                if (UseLineModel)
                {
                    line.RichViewInfo = this;
                    line.Measure();
                    CursorMoveNextLine();
                }
            };
            AddLine();
        }
        /// <summary>
        /// 光标向下移动一行
        /// </summary>
        public void CursorMoveNextLine()
        {
            if (CursorLine < ContentLines.Count - 1)
                CursorLine++;
        }
        /// <summary>
        /// 光标向上移动一行
        /// </summary>
        public void CursorMovePreviousLine()
        {
            if (CursorLine > 0)
                CursorLine--;
        }

        /// <summary>
        /// 改变页面大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ChangePageSize(int width,int height)
        { 
            Layout.PageSize = new SizeF((float)width, (float)height);
            CreateBuffGraphics(width,height);
            if (UseLineModel)
            {
                ContentLines.ForEach((line) =>
                {
                    line.Measure();
                });
            }
                
            if (!UseLineModel)
            {
                ContentItems.ForEach((item) =>
                {
                    item.CalculationLocation();
                });
            }
                
        }

        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">插入位置的索引</param>
        /// <param name="item">内容元素</param>
        public void InsertItem(int index, IContentItem item)
        {
            ContentItems.Insert(index,item);
        }

        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">插入位置的索引</param>
        /// <param name="items">内容元素集合</param>
        public void InsertRangeItem(int index, IEnumerable<IContentItem> items)
        {
            ContentItems.InsertRange(index, items);
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <param name="item">内容元素</param>
        public void AddItem(IContentItem item)
        {
            ContentItems.Add(item);
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <param name="items">内容元素集合</param>
        public void AddRangeItem(IEnumerable<IContentItem> items)
        {
            ContentItems.AddRange(items);
        }

        /// <summary>
        /// 追加一行,并添加Item
        /// </summary>
        /// <param name="items"></param>
        public void AddLine(IEnumerable<IContentItem> items = null)
        {
            ContentLines.Add(new ContentLine());
            var newLine = ContentLines.Last();
            newLine.RichViewInfo = this;
            if (items!=null && items.Count()>0)
                newLine.AddItems(items);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Styles.Clear();
            Styles = null;
            ContentItems.Dispose();
        }
    }
}
