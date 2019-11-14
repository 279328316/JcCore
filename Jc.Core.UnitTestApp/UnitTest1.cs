using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jc.Core.UnitTestApp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            EmitTest.ILGenerateSetValueMethodContent<UserDto>();
        }
    }
}
