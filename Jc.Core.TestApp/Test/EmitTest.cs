using Jc.Data.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class EmitTest
    {
        public void Test()
        {
            EmitGenerateCodeTest<UserDto>();
        }

        /// <summary>
        /// IL生成SetValueMethod内容
        /// 独立出来为共用代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="il"></param>
        private void EmitGenerateCodeTest<T>()
        {
        }
    }
}
