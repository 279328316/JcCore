using System;
using System.Data;
using Jc;

namespace Jc.Core.FrameworkTest
{
    /// <summary>
    /// User Dto
    /// </summary>
    [Table(Name = "t_User", DisplayText = "",AutoCreate = true)]
    public class DrUserDto
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
        /// 是否删除
        /// </summary>
        [Field(DisplayText = "是否删除", FieldType = "bit")]
        public bool IsDelete { get; set; }

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

        public DrUserDto()
        { 
        }

        public DrUserDto(DataRow dataRow)
        {
            UserDto userDto = new UserDto();
            DataColumnCollection columns = dataRow.Table.Columns;
            DataColumn dataColumn = null;
            try
            {
                if (columns.Contains("Id") && !dataRow.IsNull("Id"))
                {
                    dataColumn = columns["Id"];
                    userDto.Id = (int?)dataRow[dataColumn];
                }
                if (columns.Contains("UserName") && !dataRow.IsNull("UserName"))
                {
                    dataColumn = columns["UserName"];
                    userDto.UserName = (string)dataRow[dataColumn];
                }
                if (columns.Contains("UserPwd") && !dataRow.IsNull("UserPwd"))
                {
                    dataColumn = columns["UserPwd"];
                    userDto.UserPwd = (string)dataRow[dataColumn];
                }
                if (columns.Contains("NickName") && !dataRow.IsNull("NickName"))
                {
                    dataColumn = columns["NickName"];
                    userDto.NickName = (string)dataRow[dataColumn];
                }
                if (columns.Contains("RealName") && !dataRow.IsNull("RealName"))
                {
                    dataColumn = columns["RealName"];
                    userDto.RealName = (string)dataRow[dataColumn];
                }
                if (columns.Contains("Email") && !dataRow.IsNull("Email"))
                {
                    dataColumn = columns["Email"];
                    userDto.Email = (string)dataRow[dataColumn];
                }
                if (columns.Contains("Avatar") && !dataRow.IsNull("Avatar"))
                {
                    dataColumn = columns["Avatar"];
                    userDto.Avatar = (string)dataRow[dataColumn];
                }
                if (columns.Contains("PhoneNo") && !dataRow.IsNull("PhoneNo"))
                {
                    dataColumn = columns["PhoneNo"];
                    userDto.PhoneNo = (string)dataRow[dataColumn];
                }
                if (columns.Contains("Sex") && !dataRow.IsNull("Sex"))
                {
                    dataColumn = columns["Sex"];
                    if (dataColumn.DataType == typeof(int))
                    {
                        userDto.Sex = (Sex)dataRow[dataColumn];
                    }
                    else if (dataColumn.DataType == typeof(short))
                    {
                        userDto.Sex = (Sex)dataRow[dataColumn];
                    }
                    else if(dataColumn.DataType == typeof(long))
                    {
                        userDto.Sex = (Sex)dataRow[dataColumn];
                    }
                    
                }
                if (columns.Contains("Birthday") && !dataRow.IsNull("Birthday"))
                {
                    dataColumn = columns["Birthday"];
                    userDto.Birthday = (DateTime?)dataRow[dataColumn];
                }
                if (columns.Contains("IsDelete") && !dataRow.IsNull("IsDelete"))
                {
                    dataColumn = columns["IsDelete"];
                    userDto.IsDelete = (bool)dataRow[dataColumn];
                }
                if (columns.Contains("UserStatus") && !dataRow.IsNull("UserStatus"))
                {
                    dataColumn = columns["UserStatus"];
                    userDto.UserStatus = (int?)dataRow[dataColumn];
                }
                if (columns.Contains("AddUser") && !dataRow.IsNull("AddUser"))
                {
                    dataColumn = columns["AddUser"];
                    userDto.AddUser = (Guid?)dataRow[dataColumn];
                }
                if (columns.Contains("AddDate") && !dataRow.IsNull("AddDate"))
                {
                    dataColumn = columns["AddDate"];
                    userDto.AddDate = (DateTime?)dataRow[dataColumn];
                }
                if (columns.Contains("LastUpdateUser") && !dataRow.IsNull("LastUpdateUser"))
                {
                    dataColumn = columns["LastUpdateUser"];
                    userDto.LastUpdateUser = (Guid?)dataRow[dataColumn];
                }
                if (columns.Contains("LastUpdateDate") && !dataRow.IsNull("LastUpdateDate"))
                {
                    dataColumn = columns["LastUpdateDate"];
                    userDto.LastUpdateDate = (DateTime?)dataRow[dataColumn];
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Column {0} Load Error:{1}", dataColumn.ColumnName, ex.Message);
                Exception ex2 = new Exception(message);
                throw ex2;
            }
        }

        #endregion

    }
}
