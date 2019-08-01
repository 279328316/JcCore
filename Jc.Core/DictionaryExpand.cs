using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Jc.Core
{
    /// <summary>
    /// Object 扩展
    /// </summary>
    public static class DictionaryHelper
    {

        #region ICloneable 成员

        /// <summary>
        /// 追加元素到字典
        /// </summary>
        /// <param name="dest"></param>
        public static Dictionary<TKey,TValue> Append<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key,TValue value)
        {
            if (source == null)
            {
                throw new Exception("添加元素,字段对象不能为空.");
            }
            source.Add(key, value);
            return source;
        }
        #endregion
    }
}
