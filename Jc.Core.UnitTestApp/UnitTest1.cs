using System;
using Jc.Core.UnitTestApp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jc.Core.UnitTestApp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertDtoTest()
        {
            DataRowToDtoTest.Test();
        }

        [TestMethod]
        public void EmitCreateTest()
        {
            EmitTest.ILGenerateSetValueMethodContent<UserDto>();
        }
    }
}
