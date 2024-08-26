using System;
using System.Data;
using Jc;

namespace Jc.Core.TestApp
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
        public UserStatus UserStatus { get; set; }

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
            DateTime? dt1 = DateTime.Now;
            if (dt1 != null)
            {
                dt1 = null;
            }
            else 
            {                 
                dt1 = DateTime.Now;
            }
            int? i = 0;
            if (i != null)
            {
                i = null;
            }
            else
            {
                i = 1;
            }
            double? a = null;
            if (a == null)
            {
                a = i;
            }
            string str = null;
            if (str == null)
            {
                str = "id";
            }
            a = default;
            i = default;
            dt1 = default;
            str = null;
        }

        public DrUserDto(DataRow dataRow)
        {
            DataColumnCollection columns = dataRow.Table.Columns;
            DataColumn dataColumn = null;
            try
            {
                if (columns.Contains("Id"))
                {
                    dataColumn = columns["Id"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Id = (int?)dataRow[dataColumn];
                    }
                    else
                    {
                        Id = null;
                    }
                }
                if (columns.Contains("UserName"))
                {
                    dataColumn = columns["UserName"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        UserName = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        UserName = null;
                    }
                }
                if (columns.Contains("UserPwd"))
                {
                    dataColumn = columns["UserPwd"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        UserPwd = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        UserPwd = null;
                    }
                }
                if (columns.Contains("NickName"))
                {
                    dataColumn = columns["NickName"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        NickName = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        NickName = null;
                    }
                }
                if (columns.Contains("RealName"))
                {
                    dataColumn = columns["RealName"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        RealName = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        RealName = null;
                    }
                }
                if (columns.Contains("Email"))
                {
                    dataColumn = columns["Email"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Email = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        Email = null;
                    }
                }
                if (columns.Contains("Avatar"))
                {
                    dataColumn = columns["Avatar"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Avatar = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        Avatar = null;
                    }
                }
                if (columns.Contains("PhoneNo"))
                {
                    dataColumn = columns["PhoneNo"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        PhoneNo = (string)dataRow[dataColumn];
                    }
                    else
                    {
                        PhoneNo = null;
                    }
                }
                if (columns.Contains("Sex"))
                {
                    dataColumn = columns["Sex"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Sex = (Sex)dataRow[dataColumn];
                    }
                    else
                    {
                        Sex = null;
                    }
                }
                if (columns.Contains("Birthday"))
                {
                    dataColumn = columns["Birthday"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Birthday = (DateTime?)dataRow[dataColumn];
                    }
                    else
                    {
                        Birthday = (DateTime?)dataRow[dataColumn];
                        DateTime ? a = null;
                        Birthday = a;
                    }
                }
                if (columns.Contains("IsDelete"))
                {
                    dataColumn = columns["IsDelete"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        IsDelete = (bool)dataRow[dataColumn];
                    }
                    else
                    {
                        throw new Exception("field value is null");
                    }
                }
                if (columns.Contains("UserStatus"))
                {
                    dataColumn = columns["UserStatus"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        UserStatus = (UserStatus)dataRow[dataColumn];
                    }
                    else
                    {
                        throw new Exception("field value is null");
                    }
                }
                if (columns.Contains("AddUser"))
                {
                    dataColumn = columns["AddUser"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        AddUser = (Guid?)dataRow[dataColumn];
                    }
                    else
                    {
                        Guid? a = null;
                        AddUser = a;
                    }
                }
                if (columns.Contains("AddDate"))
                {
                    dataColumn = columns["AddDate"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        AddDate = (DateTime?)dataRow[dataColumn];
                    }
                    else
                    {
                        DateTime? a = null;
                        AddDate = a;
                    }
                }
                if (columns.Contains("LastUpdateUser"))
                {
                    dataColumn = columns["LastUpdateUser"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        LastUpdateUser = (Guid?)dataRow[dataColumn];
                    }
                    else
                    {
                        LastUpdateUser = null;
                    }
                }
                if (columns.Contains("LastUpdateDate"))
                {
                    dataColumn = columns["LastUpdateDate"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        LastUpdateDate = (DateTime?)dataRow[dataColumn];
                    }
                    else
                    {
                        LastUpdateDate = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Column {0} load error , {1}", dataColumn.ColumnName, ex.Message);
                Exception ex2 = new Exception(message);
                throw ex2;
            }
        }


        public DrUserDto(DataRow dataRow,int index)
        {
            DrUserDto dto = new DrUserDto();
            DataColumnCollection columns = dataRow.Table.Columns;
            DataColumn dataColumn = null;
            try
            {
                if (columns.Contains("Sex"))
                {
                    dataColumn = columns["Sex"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        Sex = (Sex?)dataRow[dataColumn];
                    }
                    else
                    {
                        Sex = null;
                    }
                }
                if (columns.Contains("IsDelete"))
                {
                    dataColumn = columns["IsDelete"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        IsDelete = (bool)dataRow[dataColumn];
                    }
                    else
                    {
                        throw new Exception("field value is null");
                    }
                }
                if (columns.Contains("UserStatus"))
                {
                    dataColumn = columns["UserStatus"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        UserStatus = (UserStatus)dataRow[dataColumn];
                    }
                    else
                    {
                        throw new Exception("field value is null");
                    }
                }
                if (columns.Contains("AddUser"))
                {
                    dataColumn = columns["AddUser"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        AddUser = (Guid?)dataRow[dataColumn];
                    }
                    else
                    {
                        AddUser = null;
                    }
                }
                if (columns.Contains("AddDate"))
                {
                    dataColumn = columns["AddDate"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        AddDate = (DateTime?)dataRow[dataColumn];
                    }
                    else
                    {
                        AddDate = null;
                    }
                }
                if (columns.Contains("LastUpdateUser"))
                {
                    dataColumn = columns["LastUpdateUser"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        LastUpdateUser = (Guid?)dataRow[dataColumn];
                    }
                    else
                    {
                        LastUpdateUser = null;
                    }
                }
                if (columns.Contains("LastUpdateDate"))
                {
                    dataColumn = columns["LastUpdateDate"];
                    if (!dataRow.IsNull(dataColumn))
                    {
                        LastUpdateDate = (DateTime?)dataRow[dataColumn];
                    }
                    else
                    {
                        LastUpdateDate = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Column {0} load error , {1}", dataColumn.ColumnName, ex.Message);
                Exception ex2 = new Exception(message);
                throw ex2;
            }
        }
        #endregion

    }
}
