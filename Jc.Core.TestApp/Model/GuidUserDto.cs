using System;
using Jc.Core;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// User Dto
    /// </summary>
    [Table(Name = "t_GUser{0}", DisplayText = "",AutoCreate = true)]
    public class GUserDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Field(DisplayText = "Id", IsPk = true, Required = true, FieldType = "uniqueidentifier")]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Field(DisplayText = "用户名", FieldType = "varchar", Length = 50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Field(DisplayText = "密码", FieldType = "varchar", Length = 50)]
        public string UserPwd { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Field(DisplayText = "昵称", FieldType = "varchar", Length = 50)]
        public string NickName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Field(DisplayText = "真实姓名", FieldType = "varchar", Length = 50)]
        public string RealName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Field(DisplayText = "Email", FieldType = "varchar", Length = 50)]
        public string Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Field(DisplayText = "头像", FieldType = "varchar", Length = 300)]
        public string Avatar { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [Field(DisplayText = "联系方式", FieldType = "varchar", Length = 100)]
        public string PhoneNo { get; set; }

        /// <summary>
        /// 性别 0女 1男
        /// </summary>
        [Field(DisplayText = "性别 0女 1男", FieldType = "int")]
        public int? Sex { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Field(DisplayText = "出生日期", FieldType = "datetime")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        [Field(DisplayText = "微信OpenId", FieldType = "varchar", Length = 50)]
        public string WeChatOpenId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Field(DisplayText = "是否删除", FieldType = "bit")]
        public bool? IsDelete { get; set; }

        /// <summary>
        /// 状态 0停用 1启用
        /// </summary>
        [Field(DisplayText = "状态 0停用 1启用", FieldType = "int")]
        public int? UserStatus { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        [Field(DisplayText = "添加人", FieldType = "uniqueidentifier")]
        public Guid? AddUser { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        [Field(DisplayText = "添加日期", FieldType = "datetime")]
        public DateTime? AddDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [Field(DisplayText = "修改人", FieldType = "uniqueidentifier")]
        public Guid? LastUpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Field(DisplayText = "修改时间", FieldType = "datetime")]
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}
