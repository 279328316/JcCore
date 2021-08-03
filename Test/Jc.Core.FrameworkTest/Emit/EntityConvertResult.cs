using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Jc.Core.FrameworkTest
{
    /// <summary>
    /// Entity Convert Result
    /// </summary>
    public class EntityConvertResult
    {
        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception Detail
        /// </summary>
        public Exception Exception { get; set; }
    }
}