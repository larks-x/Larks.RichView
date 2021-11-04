using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichView.ElementObject
{
    /// <summary>
    /// 元素唯一键
    /// </summary>
    public class ElementKey
    {
        private long _Key = 0;
        /// <summary>
        /// 唯一键
        /// </summary>
        [JsonIgnore]
        public long UniqueKey
        {
            get {
                if (_Key == 0)
                    _Key = GenerateKey();
                return _Key;
            }
        }

        /// <summary>
        /// 生成Key
        /// </summary>
        /// <returns></returns>
        private long GenerateKey()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
