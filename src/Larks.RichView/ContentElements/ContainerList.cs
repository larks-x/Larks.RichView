namespace Larks.RichView.ContentElements
{
    public class ContainerList<T> : IEnumerable<T>, IEnumerable where T : IContentItem
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
        /// 删除数据
        /// </summary>
        public Action<List<T>> ItemRemove;
        public int Count => list.Count;
        public void Add(T obj)
        {
            list.Add(obj);
            ItemAdd?.Invoke(obj);
        }

        public void AddRange(IEnumerable<T> objs)
        {
            list.AddRange(objs);
            ItemAddRange?.Invoke(objs);
        }

        public void Insert(int index, T obj)
        {
            list.Insert(index, obj);
            ItemAdd?.Invoke(obj);
        }

        public void InsertRange(int index, IEnumerable<T> objs)
        {
            list.InsertRange(index, objs);
            ItemAddRange?.Invoke(objs);
        }

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
            return array.ToList();

        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

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
        /// 
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

    }
}
