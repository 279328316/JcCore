﻿using System;
using Jc.Core;

namespace Jc.Core.TestApp.PetCt
{
    public enum SexType
    {
        F = 0,
        M = 1,
        O = 2
    }

    /// <summary>
    /// 患者信息 Dto
    /// </summary>
    [Table(Name ="tbl_patient",DisplayText ="患者信息")]
    public class PetCt_PatientDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 患者编号
        /// </summary>
        public string Patient_id { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public SexType? Sex { get; set; }
        //public int? Sex { get; set; }

        /// <summary>
        /// 年龄组
        /// </summary>
        /// 
        //public AgeGroupType Age_Group { get; set; }
        public int? Age_group { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birth_date { get; set; }

        /// <summary>
        /// 体重(kg)
        /// </summary>
        public float? Weight { get; set; }

        /// <summary>
        /// 身高(cm)
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        /// 病史
        /// </summary>
        public string History { get; set; }

        /// <summary>
        /// 糖尿病
        /// </summary>
        public int? Diabetic { get; set; }
        //public DiabeticType? Diabetic { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 最近治疗时间
        /// </summary>
        public DateTime? Last_treatment_date { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? Last_modified_date_time { get; set; }

        #endregion
    }
}
