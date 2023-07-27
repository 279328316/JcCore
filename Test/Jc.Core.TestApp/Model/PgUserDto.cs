using System;
using Jc;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// Pg User Dto
    /// </summary>
    [Table(Name = "tbl_User")]
    public class PgUserDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Field(Name ="id", IsPk = true, Required = true, FieldType = "int")]
        public int? Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Field(Name = "user_name", FieldType = "varchar", Length = 50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Field(Name = "password", DisplayText = "密码", FieldType = "varchar", Length = 50)]
        public string UserPwd { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Field(Name = "first_name", DisplayText = "昵称", FieldType = "varchar", Length = 50)]
        public string FirstName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Field(Name = "last_name", DisplayText = "真实姓名", FieldType = "varchar", Length = 50)]
        public string LastName { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Field(Name = "is_super_user", DisplayText = "是否超级管理员", FieldType = "bit")]
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Field(Name = "is_actived", DisplayText = "是否激活", FieldType = "bit")]
        public bool IsActived { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Field(Name = "last_modified_date_time", DisplayText = "修改时间", FieldType = "datetime")]
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}
