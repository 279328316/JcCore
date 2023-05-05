using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.FrameworkTest
{
    public class EntityConvertorTest
    {
        public void GetListTest()
        {
            try
            {
                DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
                if (dt?.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        DrUserDto drUser = new DrUserDto(dr);
                    }
                }

                IEntityConvertor<UserDto> entityConvertor = EmitTest.GenerateEntityConvertor_ThrowEx<UserDto>();
                List<UserDto> users = new List<UserDto>();
                if (dt?.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EntityConvertResult convertResult = new EntityConvertResult();
                        DataRow dr = dt.Rows[i];
                        DrUserDto drUser = new DrUserDto(dr);
                        UserDto user = entityConvertor.ConvertDto(dr, convertResult);
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
