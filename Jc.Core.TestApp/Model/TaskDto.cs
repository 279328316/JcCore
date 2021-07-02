using System;
using Jc.Core;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// Task Dto
    /// </summary>
    [Table(Name = "t_Task", DisplayText = "",AutoCreate = true)]
    public class TaskDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Field(DisplayText = "Id", IsPk = true, Required = true, FieldType = "int")]
        public int? Id { get; set; }

        /// <summary>
        /// TaskName
        /// </summary>
        [Field(DisplayText = "TaskName", FieldType = "varchar", Length = 50)]
        public string TaskName { get; set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        [Field(DisplayText = "运行次数", FieldType = "int")]
        public int? RunCount { get; set; }

        /// <summary>
        /// 最大运行次数
        /// </summary>
        [Field(DisplayText = "最大运行次数", FieldType = "int")]
        public int? MaxRunCount { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        [Field(DisplayText = "添加日期", FieldType = "datetime")]
        public DateTime? AddDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [Field(DisplayText = "修改人", FieldType = "uniqueidentifier")]
        public Guid? LastUpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Field(DisplayText = "修改时间", FieldType = "datetime")]
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}
