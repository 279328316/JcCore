using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class DbContextTest
    {
        public void Test()
        {
            GetSortedListTest();
        }

        public void GetSortedListTest()
        {
            Guid institutionId = Guid.Parse("656c3b5a-8b7d-4887-8edd-323e5fc21f9c");
            List<ReportDto> list = Dbc.YamDb.GetSortList<ReportDto>(a => a.InstitutionId == institutionId && a.AppointmentId == null, a => a.UploadDate);
        }
    }
}
