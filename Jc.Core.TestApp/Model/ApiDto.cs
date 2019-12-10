using System;
using Jc.Core;

namespace Jc.Core.TestApp
{
    /// <summary>
    /// Api Dto
    /// </summary>
    [Table(Name ="s_Api",DisplayText ="")]
    public class ApiDto
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [PkField]
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Controller
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 允许匿名访问
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// ApiLevel 级别
        /// Area 1级 Controller 2级 Action 3级
        /// </summary>
        public int? ApiLevel { get; set; }
        
        /// <summary>
        /// 标识可能已被删除
        /// </summary>
        public bool MaybeDeleted { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public Guid? AddUser { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime? AddDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public Guid? LastUpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDate { get; set; }
        #endregion
    }
}
