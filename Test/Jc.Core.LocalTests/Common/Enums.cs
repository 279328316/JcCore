using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Tests
{
    [DisplayName("性别")]
    public enum Sex
    {
        /// <summary>
        /// 女
        /// </summary>
        [DisplayName("女")]
        Female = 0,

        /// <summary>
        /// 男
        /// </summary>
        [DisplayName("男")]
        Male = 1,
    }
}
