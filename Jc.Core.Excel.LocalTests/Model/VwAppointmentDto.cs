using System;
using Jc.Core;

namespace Jc.Core.Excel.Tests
{
    /// <summary>
    /// VIEW Dto
    /// </summary>
    [Table(Name = "vw_appointment",DisplayText = "VIEW")]
    public class VwAppointmentDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [PkField]
        public Guid Id { get; set; }

        /// <summary>
        /// 预约用户Id
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 创建方式 1微信 2管理后台
        /// </summary>
        public AppointmentCreateType CreateType { get; set; }

        /// <summary>
        /// 创建方式名称
        /// </summary>
        [NoMapping]
        public string CreateTypeName { get { return CreateType.GetDisplayName(); } }

        /// <summary>
        /// 就诊人Id
        /// </summary>
        public Guid? AppointmentPersonId { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 患者性别
        /// </summary>
        public Sex PatientSex { get; set; }

        /// <summary>
        /// 性别名称
        /// </summary>
        [NoMapping]
        public string SexName { get { return PatientSex.GetDisplayName(); } }

        /// <summary>
        /// 患者出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 患者年龄
        /// </summary>
        [NoMapping]
        public int? Age 
        { 
            get 
            {
                int? age = null;
                if (Birthday.HasValue)
                {
                    age = DateTime.Now.Year - Birthday.Value.Year;
                }
                return age;
            } 
        }

        /// <summary>
        /// 身高(cm)
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// 体重(kg)
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 是否为糖尿病患者
        /// </summary>
        public bool? IsDiabetic { get; set; }

        /// <summary>
        /// 诊断
        /// </summary>
        public string Diagnosis { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public IdCardType IdCardType { get; set; }

        /// <summary>
        /// 证件类型名称
        /// </summary>
        [NoMapping]
        public string IdCardTypeName { get { return IdCardType.GetDisplayName(); } }

        /// <summary>
        /// 证件号
        /// </summary>
        public string IdCardNumber { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 常住地址
        /// </summary>
        public string CommonAddress { get; set; }

        /// <summary>
        /// 当前住址
        /// </summary>
        public string CurrentAddress { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人证件类型
        /// </summary>
        public IdCardType ContactIdCardType { get; set; }

        /// <summary>
        /// 联系人证件类型名称
        /// </summary>
        [NoMapping]
        public string ContactIdCardTypeName { get { return ContactIdCardType.GetDisplayName(); } }

        /// <summary>
        /// 联系人证件号
        /// </summary>
        public string ContactIdCardNumber { get; set; }

        /// <summary>
        /// 检查号
        /// </summary>
        public string ExamNumber { get; set; }


        /// <summary>
        /// 检查项名称
        /// </summary>
        public string ExamItemName { get; set; }

        /// <summary>
        /// 邀请Code
        /// </summary>
        public string InvitationCode { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        public DateTime? AppointmentTime { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public AppointmentState AppointmentState { get; set; }

        /// <summary>
        /// 预约状态名称
        /// </summary>
        [NoMapping]
        public string AppointmentStateName { get { return AppointmentState.GetDisplayName(); } }

        /// <summary>
        /// 检查费用
        /// </summary>
        public decimal? ExamCost { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 任务添加人Id
        /// </summary>
        public Guid? AddUserId { get; set; }

        /// <summary>
        /// 任务添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 最后更新人Id
        /// </summary>
        public Guid? LastUpdateUserId { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateDate { get; set; }
        #endregion
    }
}
