using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core.TestApp
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

    [DisplayName("性别")]
    public enum UserStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        [DisplayName("停用")]
        Disabled = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [DisplayName("启用")]
        Enabled = 1,
    }
}
