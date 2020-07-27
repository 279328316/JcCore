using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core.Excel.Tests
{
    /// <summary>
    /// 证件类型
    /// </summary>
    public enum IdCardType
    {
        /// <summary>
        /// 居民身份证
        /// </summary>
        [DisplayName(DisplayName = "居民身份证")]
        IdCard = 1,
        /// <summary>
        /// 护照
        /// </summary>
        [DisplayName(DisplayName = "护照")]
        Passport = 2,
        /// <summary>
        /// 港澳居民来往内地通行证
        /// </summary>
        [DisplayName(DisplayName = "港澳居民来往内地通行证")]
        HomeReturnPermit = 3,
        /// <summary>
        /// 台湾居民来往大陆通行证
        /// </summary>
        [DisplayName(DisplayName = "台湾居民来往大陆通行证")]
        MTPForTaiwan = 4,
        /// <summary>
        /// 外国人永久居留身份证
        /// </summary>
        [DisplayName(DisplayName = "外国人永久居留身份证")]
        PermanentResidenceForAlienIdCard = 5,
        /// <summary>
        /// 港澳居民居住证
        /// </summary>
        [DisplayName(DisplayName = "港澳居民居住证")]
        RPForHongKongAndMacao = 6,
        /// <summary>
        /// 台湾居民居住证
        /// </summary>
        [DisplayName(DisplayName = "台湾居民居住证")]
        RPForTaiwan = 7
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// 女
        /// </summary>
        [DisplayName(DisplayName = "女")]
        FeMale = 0,
        /// <summary>
        /// 男
        /// </summary>
        [DisplayName(DisplayName = "男")]
        Male = 1,
        /// <summary>
        /// 其它
        /// </summary>
        [DisplayName(DisplayName = "其它")]
        Other = 2
    }


    /// <summary>
    /// AppointmentCreateType 服务创建方式
    /// </summary>
    public enum AppointmentCreateType
    {
        /// <summary>
        /// 微信-患者预约
        /// </summary>      
        [DisplayName(DisplayName = "微信-患者预约")]
        WechatAppointment = 1,
        /// <summary>
        /// 前台登记
        /// </summary>
        [DisplayName(DisplayName = "前台登记")]
        ManageWeb = 2,
    }

    /// <summary>
    /// AppointmentState 预约状态
    /// </summary>
    public enum AppointmentState
    {
        /// <summary>
        /// 待确认
        /// </summary>      
        [DisplayName(DisplayName = "待确认")]
        WaitForConfirm = 0,
        /// <summary>
        /// 已确认
        /// </summary>      
        [DisplayName(DisplayName = "已确认")]
        Confirmed = 1,
        /// <summary>
        /// 已出报告
        /// </summary>      
        [DisplayName(DisplayName = "已出报告")]
        Complete = 2,
        /// <summary>
        /// 已取消
        /// </summary>      
        [DisplayName(DisplayName = "已取消")]
        Cancelled = 3,
    }


    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        [DisplayName(DisplayName = "普通用户")]
        CommonUser = 0,
        /// <summary>
        /// 医助
        /// </summary>
        [DisplayName(DisplayName = "医助")]
        DoctorAssistant = 1,
        /// <summary>
        /// 医生用户
        /// </summary>
        [DisplayName(DisplayName = "医生用户")]
        DoctorUser = 2,
        /// <summary>
        /// 系统用户
        /// </summary>
        [DisplayName(DisplayName = "系统用户")]
        SystemUser = 7,
        /// <summary>
        /// 系统管理员
        /// </summary>
        [DisplayName(DisplayName = "系统管理员")]
        SystemAdmin = 9,
    }

}
