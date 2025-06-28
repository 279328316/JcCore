using System;
using Jc;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// Pg User Dto
    /// </summary>
    [Table(Name = "t_user")]
    public class PgUserDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Field(Name = "id", IsPk = true, Required = true, FieldType = "int")]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Field(Name = "username", FieldType = "varchar", Length = 50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Field(Name = "userpwd", DisplayText = "密码", FieldType = "varchar", Length = 50)]
        public string UserPwd { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Field(Name = "nickname", DisplayText = "昵称", FieldType = "varchar", Length = 50)]
        public string NickName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Field(Name = "realname", DisplayText = "真实姓名", FieldType = "varchar", Length = 50)]
        public string RealName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Field(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Field(Name = "avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [Field(Name = "phoneno")]
        public string PhoneNo { get; set; }

        /// <summary>
        /// 性别 0女 1男
        /// </summary>
        [Field(Name = "sex")]
        public Sex? Sex { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Field(Name = "birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Field(Name = "isdelete")]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 状态 0停用 1启用
        /// </summary>
        [Field(Name = "userstatus")]
        public UserStatus UserStatus { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        [Field(Name = "adduser")]
        public Guid? AddUser { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        [Field(Name = "adddate")]
        public DateTime? AddDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [Field(Name = "lastupdateuser")]
        public Guid? LastUpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Field(Name = "lastupdatedate")]
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}