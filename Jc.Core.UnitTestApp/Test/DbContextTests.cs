using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.UnitTestApp
{
    [TestClass()]
    public class DbContextTests
    {
        [TestMethod()]
        public void GetListTest()
        {
            DateTime tempStartDt = DateTime.Now.Date.AddDays(-30);
            DateTime tempEndDt = DateTime.Now.Date.AddDays(1);
            IQuery<PetCt_ScanDto> query = Dbc.Db.IQuery<PetCt_ScanDto>().AutoOrderBy("Scan_date", "desc", a => new { a.Scan_date, a.Scan_time }, Sorting.Desc);
            List<PetCt_ScanDto> scans = query.Where(a => a.Is_deleted == false && a.Scan_date >= tempStartDt && a.Scan_date < tempEndDt).ToList();

            List<long> patientIds = scans.Select(a => a.Scan_patient_id).Distinct().ToList();
            List<PatientDto> patients = Dbc.Db.GetList<PatientDto>(a => patientIds.Contains(a.Id));

        }
    }
}