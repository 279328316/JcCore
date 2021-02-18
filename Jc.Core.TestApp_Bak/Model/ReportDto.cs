using System;
using Jc.Core;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// Report Dto
    /// </summary>
    [Table(Name = "t_report")]
    public class ReportDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [PkField]
        public Guid Id { get; set; }

        /// <summary>
        /// 检查中心Id
        /// </summary>
        public Guid? InstitutionId { get; set; }

        /// <summary>
        /// 关联预约Id
        /// </summary>
        public Guid? AppointmentId { get; set; }

        /// <summary>
        /// 报告患者姓名
        /// </summary>
        public string ReportPatientName { get; set; }

        /// <summary>
        /// 报告检查编号
        /// </summary>
        public string ReportExamNumber { get; set; }

        /// <summary>
        /// 报告Code
        /// </summary>
        public string ReportCode { get; set; }

        /// <summary>
        /// 报告文件Id
        /// </summary>
        public Guid? ReportFileId { get; set; }

        /// <summary>
        /// 报告文件名称
        /// </summary>
        public string ReportFileName { get; set; }

        /// <summary>
        /// 报告文件Url
        /// </summary>
        public string ReportFileUrl { get; set; }

        /// <summary>
        /// Dicom文件Id
        /// </summary>
        public Guid? DicomFileId { get; set; }

        /// <summary>
        /// Dicom文件名称
        /// </summary>
        public string DicomFileName { get; set; }

        /// <summary>
        /// Dicom文件Url
        /// </summary>
        public string DicomFileUrl { get; set; }

        /// <summary>
        /// 下载次数
        /// </summary>
        public int? DownloadAmount { get; set; }

        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime? UploadDate { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime? AddDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}
