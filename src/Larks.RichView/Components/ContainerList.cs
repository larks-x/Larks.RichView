namespace Larks.RichView.Components
{
    /// <summary>
    /// 容器用list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContainerList<T> : IEnumerable<T>, IEnumerable,IDisposable where T : IContentItem
    {
        private List<T> list = new List<T>();
        /// <summary>
        /// 添加数据
        /// </summary>
        public Action<T> ItemAdd;
        /// <summary>
        /// 添加数据
        /// </summary>
        public Action<IEnumerable<T>> ItemAddRange;
        /// <summary>
        /// 插入数据
        /// </summary>
        public Action<int,T> ItemInsert;
        /// <summary>
        /// 插入数据
        /// </summary>
        public Action<int,IEnumerable<T>> ItemInsertRange;
        /// <summary>
        /// 删除数据
        /// </summary>
        public Action<List<T>> ItemRemove;
        /// <summary>
        /// 数据改变事件
        /// </summary>
        public Action ItemChange;
        /// <summary>
        /// 数量
        /// </summary>
        public int Count => list.Count;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj)
        {
            list.Add(obj);
            ItemAdd?.Invoke(obj);
            ItemChange?.Invoke();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void AddRange(IEnumerable<T> objs)
        {
            list.AddRange(objs);
            ItemAddRange?.Invoke(objs);
            ItemChange?.Invoke();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        public void Insert(int index, T obj)
        {
            list.Insert(index, obj);
            ItemInsert?.Invoke(index,obj);
            ItemChange?.Invoke();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objs"></param>
        public void InsertRange(int index, IEnumerable<T> objs)
        {
            list.InsertRange(index, objs);
            ItemInsertRange?.Invoke(index,objs);
            ItemChange?.Invoke();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int IndexOf(T obj)
        {
            return list.IndexOf(obj);
        }
        
        /// <summary>
        /// 弹出指定区域的数据
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<T> PopRange(int index, int count)
        {
            T[] array = new T[count];
            Array.Copy(list.ToArray(), index, array, 0, count);
            list.RemoveRange(index, count);
            ItemChange?.Invoke();
            return array.ToList();
        }

        /// <summary>
        /// 弹出指定位置之后的所有数据
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <returns></returns>
        public List<T> PopRange(int index)
        {
            int count = list.Count - index;
            T[] array = new T[count];
            Array.Copy(list.ToArray(), index, array, 0, count);
            list.RemoveRange(index, count);
            ItemChange?.Invoke();
            return array.ToList();

        }
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            for (int i = 0; i < Count; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                list[index] = value;

            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// List克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ContainerList<T> Clone()
        {
            var newlist = new ContainerList<T>();
            newlist.list = list.Select(item => (T)item.Clone()).ToList();
            return newlist;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Clear();
            list = null;
        }
    }
}
