
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc
{
    /// <summary>
    /// IEntity base class
    /// Entity Class请使用 IEntity<T>
    /// Table must use id as primary key
    /// </summary>
    public class IEntity : NotifyObject
    {
    }

    /// <summary>
    /// IEntity<T>
    /// Include Id Property As Key
    /// Table must use field id as primary key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IEntity<T> : IEntity where T : struct
    {
        public T Id { get; set; }
    }
}
