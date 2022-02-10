
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Tests.Test
{
    [TestClass]
    public class DataRowToDtoTest
    {
        [TestMethod]
        public void Test()
		{
			List<UserDto> users = new List<UserDto>();
			DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
			if (dt?.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					UserDto user = ConvertUserDto2(dt.Rows[i]);
					users.Add(user);
				}
			}
        }

		public static UserDto ConvertUserDto(DataRow dataRow)
		{
            UserDto userDto = new UserDto();
            DataColumnCollection columns = dataRow.Table.Columns;
            try
            {
                if (columns.Contains("Id") && !dataRow.IsNull("Id"))
                {
                    userDto.Id = (int?)dataRow["Id"];
                }
                if (columns.Contains("UserName") && !dataRow.IsNull("UserName"))
                {
                    userDto.UserName = (string)dataRow["UserName"];
                }
                if (columns.Contains("UserPwd") && !dataRow.IsNull("UserPwd"))
                {
                    userDto.UserPwd = (string)dataRow["UserPwd"];
                }
                if (columns.Contains("NickName") && !dataRow.IsNull("NickName"))
                {
                    userDto.NickName = (string)dataRow["NickName"];
                }
                if (columns.Contains("RealName") && !dataRow.IsNull("RealName"))
                {
                    userDto.RealName = (string)dataRow["RealName"];
                }
                if (columns.Contains("Email") && !dataRow.IsNull("Email"))
                {
                    userDto.Email = (string)dataRow["Email"];
                }
                if (columns.Contains("Avatar") && !dataRow.IsNull("Avatar"))
                {
                    userDto.Avatar = (string)dataRow["Avatar"];
                }
                if (columns.Contains("PhoneNo") && !dataRow.IsNull("PhoneNo"))
                {
                    userDto.PhoneNo = (string)dataRow["PhoneNo"];
                }
                if (columns.Contains("Sex") && !dataRow.IsNull("Sex"))
                {
                    //userDto.Sex = TEnumHelper<Sex>.ToEnum(dataRow["Sex"]);
                }
                if (columns.Contains("Birthday") && !dataRow.IsNull("Birthday"))
                {
                    userDto.Birthday = (DateTime?)dataRow["Birthday"];
                }
                if (columns.Contains("WeChatOpenId") && !dataRow.IsNull("WeChatOpenId"))
                {
                    userDto.WeChatOpenId = (string)dataRow["WeChatOpenId"];
                }
                if (columns.Contains("IsDelete") && !dataRow.IsNull("IsDelete"))
                {
                    userDto.IsDelete = (bool?)dataRow["IsDelete"];
                }
                if (columns.Contains("UserStatus") && !dataRow.IsNull("UserStatus"))
                {
                    userDto.UserStatus = (int?)dataRow["UserStatus"];
                }
                if (columns.Contains("AddUser") && !dataRow.IsNull("AddUser"))
                {
                    userDto.AddUser = (Guid?)dataRow["AddUser"];
                }
                if (columns.Contains("AddDate") && !dataRow.IsNull("AddDate"))
                {
                    userDto.AddDate = (DateTime?)dataRow["AddDate"];
                }
                if (columns.Contains("LastUpdateUser") && !dataRow.IsNull("LastUpdateUser"))
                {
                    userDto.LastUpdateUser = (Guid?)dataRow["LastUpdateUser"];
                }
                if (columns.Contains("LastUpdateDate") && !dataRow.IsNull("LastUpdateDate"))
                {
                    userDto.LastUpdateDate = (DateTime?)dataRow["LastUpdateDate"];
                }
            }
            catch (NotSupportedException ex)
            {
                throw new Exception($"NotSupportedException:{ex.Message}\r\n{ex.StackTrace}");
            }
            catch (Exception ex)
            {
                throw new Exception($"异常:{ex.Message}\r\n{ex.StackTrace}");
            }
			return userDto;
		}

        public static UserDto ConvertUserDto2(DataRow dataRow)
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
					userDto.Sex = (Sex)((int)dataRow[dataColumn]);
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
			return userDto;
		}
	}
}
