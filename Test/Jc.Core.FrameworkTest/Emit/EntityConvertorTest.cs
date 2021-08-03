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
                IEntityConvertor<UserDto> entityConvertor = EmitTest.GenerateEntityConvertor_ThrowEx<UserDto>();
                List<UserDto> users = new List<UserDto>();
                DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
                if (dt?.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EntityConvertResult convertResult = new EntityConvertResult();
                        DataRow dr = dt.Rows[i];
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
