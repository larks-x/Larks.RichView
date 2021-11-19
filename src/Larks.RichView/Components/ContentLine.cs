using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Larks.RichView.Components
{
    /// <summary>
    /// 行集合
    /// </summary>
    public class ContentLine : ICloneable, IDisposable
    {
        private object DrawLock=new object();
        private Graphics _BuffGraphics;
        private Bitmap _BuffBitmap;
        private RectangleF _DrawRectangle = new RectangleF(1f,1f,1f,1f);
        /// <summary>
        /// 绘制区域
        /// </summary>
        [JsonIgnore]
        public RectangleF DrawRectangle
        {
            get => _DrawRectangle;
            set {
                if (_DrawRectangle == value)
                    return;
                _DrawRectangle = value;
            }
        }
        /// <summary>
        /// 左边界
        /// </summary>
        [JsonIgnore]
        public float Left => _DrawRectangle.X;
        /// <summary>
        /// 上边界
        /// </summary>
        [JsonIgnore]
        public float Top
        {
            get => _DrawRectangle.Y;
            set {
                if (_DrawRectangle.Y == value)
                    return;
                _DrawRectangle = new RectangleF(_DrawRectangle.X,value, _DrawRectangle.Width, _DrawRectangle.Height);
            }
        }
        /// <summary>
        /// 设置宽度
        /// </summary>
        [JsonIgnore]
        public float Width {
            get => _DrawRectangle.Width;
            set {
                if (_DrawRectangle.Width == value)
                    return;
                _DrawRectangle = new RectangleF(_DrawRectangle.Location,new SizeF(value,_DrawRectangle.Height));
            }
        }

        /// <summary>
        /// 设置高度
        /// </summary>
        [JsonIgnore]
        public float Height
        {
            get => _DrawRectangle.Height;
            set
            {
                if (_DrawRectangle.Height == value)
                    return;
                _DrawRectangle = new RectangleF(_DrawRectangle.Location, new SizeF(_DrawRectangle.Width, value));
            }
        }
        /// <summary>
        /// 底边界
        /// </summary>
        [JsonIgnore]
        public float Bottom => _DrawRectangle.Bottom;

        private RichViewInformation _RichViewInfo = null;
        /// <summary>
        /// RichViewInfo引用
        /// </summary>
        [JsonIgnore]
        public RichViewInformation RichViewInfo
        {
            get => _RichViewInfo;
            set
            {
                if (_RichViewInfo != null)
                    return;
                _RichViewInfo = value;
                _RichViewInfo.OnDraw += (graphics) =>
                {
                    Draw();
                };
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        [JsonIgnore]
        public int No => RichViewInfo == null ? -1 : RichViewInfo.ContentLines.IndexOf(this);
        /// <summary>
        /// 行内Item
        /// </summary>
        [JsonIgnore]
        public ContainerList<IContentItem> Items = new ContainerList<IContentItem>();

        public ContentLine()
        {
            CreateBuffGraphics();
            Items.ItemAddAfter += (item) =>
            {
                item.LineNo = No;
                item.CalculationLocation();
                //Measure();
            };
            Items.ItemAddRangeAfter += (items) =>
            {
                items.ToList().ForEach((item) =>
                {
                    item.LineNo = No;
                    item.CalculationLocation();
                });
                //Measure();
            };
            Items.ItemInsertAfter += (index,item) =>
            {
                item.LineNo = No;
                item.CalculationLocation();
                //Measure();
            };
            Items.ItemInsertRangeAfter += (index,items) =>
            {
                items.ToList().ForEach((item) =>
                {
                    item.LineNo = No;
                    item.Measure();
                    item.CalculationLocation();
                });
                Measure();
            };
        }

        /// <summary>
        /// 向行尾追加Item
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IContentItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// 向行尾追加Item
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(IEnumerable<IContentItem> items)
        {
            Items.AddRange(items);
        }

        /// <summary>
        /// 在指定位置插入Item
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="item"></param>
        public void Insert(int index,IContentItem item)
        {
            Items.Insert(index,item);
        }

        /// <summary>
        /// 在指定位置插入Item
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="items"></param>
        public void Insert(int index, IEnumerable<IContentItem> items)
        {
            Items.InsertRange(index, items);
        }

        /// <summary>
        /// 弹出指定位置的Item
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">为0时，弹出Index后所有元素</param>
        /// <returns></returns>
        public List<IContentItem> PopItems(int index,int count = 0)
        {
            List<IContentItem> popList;
            if (count > 0)
                popList= Items.PopRange(index, count);
            else
                popList=Items.PopRange(index);
            return popList;
        }

        /// <summary>
        /// 前一个
        /// </summary>
        /// <returns></returns>
        public ContentLine? Previous()
        {
            if (No == 0)
                return null;
            return RichViewInfo.ContentLines[No - 1];
        }
        /// <summary>
        /// 后一个
        /// </summary>
        /// <returns></returns>
        public ContentLine? Next()
        {
            if (No == RichViewInfo.ContentLines.Count - 1)
                return null;
            return RichViewInfo.ContentLines[No + 1];
        }

        /// <summary>
        /// 测量宽度高度并确定位置
        /// </summary>
        public void Measure()
        {
            if (RichViewInfo == null)
                return;
            if (Width != RichViewInfo.Layout.PageSize.Width - RichViewInfo.Layout.Padding.Left - RichViewInfo.Layout.Padding.Right)
            {
                Width = RichViewInfo.Layout.PageSize.Width - RichViewInfo.Layout.Padding.Left - RichViewInfo.Layout.Padding.Right;
                //CreateBuffGraphics();
            }
            if (No > 0)
            {
                if (Top != RichViewInfo.ContentLines[No - 1].Bottom + RichViewInfo.Layout.RowSpacing)
                    Top = RichViewInfo.ContentLines[No - 1].Bottom + RichViewInfo.Layout.RowSpacing;
            }
            int i = 0;
            bool MoveNextItem = false;
            foreach (var t in Items)
            {
                if (t.DrawRectangle.Right > Width)
                {
                    MoveNextItem = true;
                    break;
                }
                i++;
            }

            if (MoveNextItem)
            {

                var moveItems = Items.PopRange(i);
                var maxHeight = Items.Max(o => o.DrawSize.Height);
                if (Height != maxHeight)
                    Height = maxHeight;
                var nextLine = Next();
                if (nextLine != null)
                {
                    nextLine.Insert(0, moveItems);
                    RichViewInfo.CursorMoveNextLine();
                }
                else
                {
                    RichViewInfo.AddLine(moveItems);
                }
            }
            else
            {
                if (Items.Count == 0)
                    return;
                var maxHeight= Items.Max(o => o.DrawSize.Height);

                if (Height != maxHeight)
                {
                    Height = maxHeight;
                    Items.ForEach((item) =>
                    {
                        item.CalculationLocation();
                    });
                }
            }
            DrawItem();

        }
        /// <summary>
        /// 创建缓存画布
        /// </summary>
        private void CreateBuffGraphics()
        {
            if (_BuffBitmap != null && _BuffBitmap.Width == (int)Width && _BuffBitmap.Height == (int)Height)
                return;
            if (_BuffBitmap != null)
            {
                _BuffGraphics?.Dispose();
                _BuffBitmap = null;
            }
           
            _BuffBitmap = new Bitmap((int)Width==0?1:(int)Width, (int)Height==0?1:(int)Height);
            _BuffGraphics = Graphics.FromImage(_BuffBitmap);
            _BuffGraphics.SmoothingMode = SmoothingMode.HighQuality;
            _BuffGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Items.ForEach((item) =>
            {
                item.CalculationLocation();
            });
        }
        /// <summary>
        /// 绘制元素到buff
        /// </summary>
        internal void DrawItem()
        {
            lock (DrawLock)
            {
                CreateBuffGraphics();
                _BuffGraphics.Clear(Color.Transparent);
                Items.ForEach((item) =>
                {
                    item.Draw(_BuffGraphics);
                });
            }
            
        }
        /// <summary>
        /// 绘制行
        /// </summary>
        public void Draw()
        {
            lock (DrawLock)
            {
                RichViewInfo?.BuffGraphics?.DrawImage(_BuffBitmap,DrawRectangle);
                //RichViewInfo?.BuffGraphics?.DrawImage(_BuffBitmap, DrawRectangle.Location);
            }
          
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
