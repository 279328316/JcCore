
using Jc.Core.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.UnitTestApp.Test
{
    public class DataRowToDtoTest
    {
        public static void Test()
		{
			List<UserDto> users = new List<UserDto>();
			DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
			if (dt?.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					UserDto user = ConvertUserDto(dt.Rows[i]);
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
            catch(Exception ex)
            {
                throw new Exception($"异常:{ex.Message}\r\n{ex.StackTrace}");
            }
			return userDto;
		}
	}
}
