using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.FrameworkTest.Dto
{
    public class IdDtoBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [Field(DisplayText = "Id", IsPk = true, Required = true, FieldType = "int")]
        public int? Id { get; set; }
    }
}
