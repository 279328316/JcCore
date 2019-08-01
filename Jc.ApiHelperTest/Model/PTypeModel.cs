using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.ApiHelperTest
{
    /// <summary>
    /// 参数对象
    /// </summary>
    public class PTypeModel
    {
        #region Properties

        /// <summary>
        /// 模块路径 类型:全路径 命名空间+所在类+属性名
        /// 例如:P:Jc.CodeCreator.Api.Controllers.PTypeModel.Id
        /// </summary>
        public string Id { get; set; }
                
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类型全称
        /// </summary>
        [JsonIgnore]
        public string TypeFullName { get { return SourceType.FullName; } }

        /// <summary>
        /// 是否为枚举类型
        /// </summary>
        public bool IsEnum { get { return SourceType.IsEnum; } }

        /// <summary>
        /// 功能组件名称 dll名称
        /// </summary>
        [JsonIgnore]
        public string ModuleName { get { return SourceType.Module.Name; } }

        /// <summary>
        /// 注释,备注
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// 属性列表
        /// </summary>
        public List<ParamModel> PiList { get; set; }

        /// <summary>
        /// 类型信息对象
        /// </summary>
        [JsonIgnore]
        public Type SourceType { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public PTypeModel()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public PTypeModel(Type type)
        {
            SourceType = type;
            Id = TypeHelper.GetModuleMark(type);
            TypeName = TypeHelper.GetTypeName(type);
        }
        #endregion
    }

}
