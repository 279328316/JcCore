using System;
using System.Data;
using Jc.Core;

namespace Jc.Tests
{
    /// <summary>
    /// User Dto
    /// </summary>
    [Table(Name = "t_User", DisplayText = "",AutoCreate = true)]
    public class UserDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Field(DisplayText = "Id", IsPk = true, Required = true, FieldType = "int")]
        public int? Id { get; set; }

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
        public Sex? Sex { get; set; }

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

        #region Ctor

        public UserDto()
        {
        }

        public UserDto(DataRow dataRow)
        {
            DataColumnCollection columns = dataRow.Table.Columns;
            if (columns.Contains("Id") && !dataRow.IsNull("Id"))
            {
                if (!(dataRow["Id"] is int))
                {
                    throw new Exception("属性类型不一致");
                }
                Id = (int?)dataRow["Id"];
            }
            if (columns.Contains("UserName") && !dataRow.IsNull("UserName"))
            {
                if (!(dataRow["UserName"] is string))
                {
                    throw new Exception("属性类型不一致");
                }
                UserName = (string)dataRow["UserName"];
            }
            if (columns.Contains("UserPwd") && !dataRow.IsNull("UserPwd"))
            {
                if (!(dataRow["UserPwd"] is string))
                {
                    throw new Exception("属性类型不一致");
                }
                UserPwd = (string)dataRow["UserPwd"];
            }
            if (columns.Contains("NickName") && !dataRow.IsNull("NickName"))
            {
                NickName = (string)dataRow["NickName"];
            }
            if (columns.Contains("RealName") && !dataRow.IsNull("RealName"))
            {
                RealName = (string)dataRow["RealName"];
            }
            if (columns.Contains("Email") && !dataRow.IsNull("Email"))
            {
                Email = (string)dataRow["Email"];
            }
            if (columns.Contains("Avatar") && !dataRow.IsNull("Avatar"))
            {
                Avatar = (string)dataRow["Avatar"];
            }
            if (columns.Contains("PhoneNo") && !dataRow.IsNull("PhoneNo"))
            {
                PhoneNo = (string)dataRow["PhoneNo"];
            }
            if (columns.Contains("Sex") && !dataRow.IsNull("Sex"))
            {
                if (!(dataRow["Sex"] is Sex))
                {
                    throw new Exception("属性类型不一致");
                }
                Sex = (Sex)dataRow["Sex"];
            }
            if (columns.Contains("Birthday") && !dataRow.IsNull("Birthday"))
            {
                if (!(dataRow["Birthday"] is DateTime))
                {
                    throw new Exception("属性类型不一致");
                }
                Birthday = (DateTime?)dataRow["Birthday"];
            }
            if (columns.Contains("WeChatOpenId") && !dataRow.IsNull("WeChatOpenId"))
            {
                WeChatOpenId = (string)dataRow["WeChatOpenId"];
            }
            if (columns.Contains("IsDelete") && !dataRow.IsNull("IsDelete"))
            {
                if (!(dataRow["IsDelete"] is bool))
                {
                    throw new Exception("属性类型不一致");
                }
                IsDelete = (bool?)dataRow["IsDelete"];
            }
            if (columns.Contains("UserStatus") && !dataRow.IsNull("UserStatus"))
            {
                UserStatus = (int?)dataRow["UserStatus"];
            }
            if (columns.Contains("AddUser") && !dataRow.IsNull("AddUser"))
            {
                if (!(dataRow["AddUser"] is Guid))
                {
                    throw new Exception("属性类型不一致");
                }
                AddUser = (Guid?)dataRow["AddUser"];
            }
            if (columns.Contains("AddDate") && !dataRow.IsNull("AddDate"))
            {
                AddDate = (DateTime?)dataRow["AddDate"];
            }
            if (columns.Contains("LastUpdateUser") && !dataRow.IsNull("LastUpdateUser"))
            {
                LastUpdateUser = (Guid?)dataRow["LastUpdateUser"];
            }
            if (columns.Contains("LastUpdateDate") && !dataRow.IsNull("LastUpdateDate"))
            {
                LastUpdateDate = (DateTime?)dataRow["LastUpdateDate"];
            }
        }

        #endregion
    }
}
