using System;
using System.Collections.Generic;
using System.Text;

namespace Jc
{
    /// <summary>
    /// 结果返回后委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">返回结果</param>
    /// <returns></returns>
    public delegate void AfterDelegate<T>(T result);

    /// <summary>
    /// After 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AfterEvent<T>
    {
        /// <summary>
        /// After
        /// </summary>
        public AfterDelegate<T> After { get; set; }

        /// <summary>
        /// 执行After
        /// </summary>
        /// <param name="result"></param>
        public void DoAfter(T result)
        {
            After?.Invoke(result);
        }
    }
}
