using Jc.Core.TestApp.PetCt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// 枚举字段添加
    /// </summary>
    public class EnumFieldTest
    {
        public void Test()
        {
            int i = 9999;
            UserDto userDto = new UserDto()
            {
                UserName = $"TestUserName{i}",
                UserPwd = $"TUserPwd{i}",
                NickName = $"TNickName{i}",
                RealName = $"TRealName{i}",
                Email = $"TEmail{i}@qq.com",
                Avatar = $"TAvatar{i}",
                PhoneNo = $"133810{i}".PadRight(11, '0'),
                Sex = (Sex)Enum.Parse(typeof(Sex), (i % 2).ToString()),
                Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i),
                WeChatOpenId = $"WeChatOpenId{i}",
                IsDelete = i % 2 == 0 ? true : false,
                UserStatus = i % 2,
                AddUser = Guid.NewGuid(),
                AddDate = DateTime.Now,
                LastUpdateUser = Guid.NewGuid(),
                LastUpdateDate = DateTime.Now
            };
            //Dbc.Db.Set(userDto);

            UserDto user = Dbc.Db.Get<UserDto>();
            Console.WriteLine(user.UserName);
        }

        public void EnumPropertyTest()
        {
            List<PetCt_PatientDto> patients = Dbc.PetCtDb.GetList<PetCt_PatientDto>();
            List<PetCt_PatientDto> femalePatients = patients.Where(a => a.Sex == SexType.F).ToList();
            List<PetCt_PatientDto> malePatients = patients.Where(a => a.Sex == SexType.M).ToList();
            List<PetCt_PatientDto> oPatients = patients.Where(a => a.Sex == SexType.O).ToList();
        }
    }
}
