using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jc.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValueObj<TKey,TValue>
    {
        /// <summary>
        /// Key
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValueObj(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public KeyValueObj()
        {
        }
    }
}
