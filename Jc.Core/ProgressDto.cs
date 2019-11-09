using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core
{
    /// <summary>
    /// 进度通知
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportingProgress<T> : Progress<T>, IProgress<T>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ReportingProgress()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ReportingProgress(Action<T> handler)
            : base(handler)
        {
        }

        /// <summary>
        /// Report
        /// </summary>
        public void Report(T value)
        {
            this.OnReport(value);
        }
    }
}
