using System;
using Jc.Base;
using Jc.Core;

namespace Jc.Core.UnitTestApp
{
    /// <summary>
    /// Scan Dto
    /// </summary>
    [Table(Name ="tbl_scan",DisplayText ="")]
    public class PetCt_ScanDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [PkField]
        public long Id { get; set; }

        /// <summary>
        /// 患者Id
        /// </summary>
        public long Scan_patient_id { get; set; }

        /// <summary>
        /// 检查Id
        /// </summary>
        public long Scan_study_id { get; set; }

        /// <summary>
        /// 检查数据类型.CT,PET
        /// </summary>
        public short Modality { get; set; }

        /// <summary>
        /// 体位侧躺方向 左,右
        /// </summary>
        public short? Laterality { get; set; }

        /// <summary>
        /// 扫描日期
        /// </summary>
        public DateTime Scan_date { get; set; }

        /// <summary>
        /// 扫描时间
        /// </summary>
        public TimeSpan Scan_time { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? Last_modified_date_time { get; set; }

        /// <summary>
        /// 工作目录
        /// </summary>
        public string Workspace { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Is_deleted { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>
        public string Protocol { get; set; }
        #endregion
    }
}
