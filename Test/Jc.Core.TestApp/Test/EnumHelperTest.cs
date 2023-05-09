
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// 枚举字段添加
    /// </summary>
    public class EnumHelperTest
    {
        public void Test()
        {
            List<EnumItem> enumItems = EnumHelper.GetEnumItems<Sex>();
            List<EnumItem> enumItems1 = EnumHelper.GetEnumItems<Sex>(a=>a.Value, Sorting.Asc);
            List<EnumItem> enumItems2 = EnumHelper.GetEnumItems<Sex>("--请选择--");
            List<EnumItem> enumItems3 = EnumHelper.GetEnumItems<Sex>("--请选择--",a => a.Value, Sorting.Asc);

            List<EnumItem> enumItems4 = EnumHelper.GetEnumItems(typeof(Sex),"--请选择--", a => a.Value, Sorting.Desc);

            Console.WriteLine("Test Finished");
        }

        public void EnumPropertyTest()
        {
        }
    }
}
