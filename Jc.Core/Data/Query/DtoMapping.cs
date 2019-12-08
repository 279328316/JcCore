using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Data.Query
{
    /// <summary>
    /// 对象参数
    /// 解析后的 tableAttr 与 fieldMapList
    /// </summary>
    public class DtoMapping
    {
        private TableAttribute tableAttr;   //Table Attr
        private Dictionary<string, PiMap> piMapDic = new Dictionary<string, PiMap>();    //piMapDic
        private PiMap pkMap; //pkMap

        private object entityConvertor;    //实际值为 EntityConvertorDelegate<T>

        /// <summary>
        /// Ctor
        /// </summary>
        public DtoMapping()
        {
        }

        /// <summary>
        /// 表对象名称
        /// </summary>
        private string TableName
        {
            get { return TableAttr?.Name; }
        }

        /// <summary>
        /// 类属性名与属性,字段关系映射列表
        /// </summary>
        public Dictionary<string, PiMap> PiMapDic
        {
            get { return piMapDic; }
            set
            {
                piMapDic = value;
            }
        }

        /// <summary>
        /// 主键字段映射Map
        /// </summary>
        public PiMap PkMap
        {
            get
            {
                if (pkMap == null)
                {
                    KeyValuePair<string, PiMap> pkFieldPair = piMapDic.Where(map => map.Value.FieldAttr.IsPk == true).FirstOrDefault();
                    if (pkFieldPair.Key != null)
                    {
                        pkMap = piMapDic[pkFieldPair.Key];
                    }
                    else
                    {
                        pkFieldPair = piMapDic.Where(map => map.Key.ToLower() == "id").FirstOrDefault();
                        if (pkFieldPair.Key != null)
                        {
                            pkMap = piMapDic[pkFieldPair.Key];
                            pkMap.FieldAttr.IsPk = true;
                        }
                        else 
                        {
                            throw new Exception($"未为表{TableName}设置主键.");
                        }
                    }
                }
                return pkMap;
            }
        }

        /// <summary>
        /// 主键字段映射Map
        /// </summary>
        public bool IsAutoIncrementPk
        {
            get
            {
                bool result = false;
                if (PkMap != null)
                {
                    if (PkMap.PropertyType == typeof(int) || PkMap.PropertyType == typeof(int?))
                    {   //自增Id
                        result = true;
                    }
                    else if (PkMap.PropertyType == typeof(long) || PkMap.PropertyType == typeof(long?))
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 表属性
        /// </summary>
        public TableAttribute TableAttr { get { return tableAttr; } set { tableAttr = value; } }

        /// <summary>
        /// EntityConvertorDelegate 实际值为 EntityConvertorDelegate(T)
        /// 使用时,再设置
        /// </summary>
        public object EntityConvertor { get => entityConvertor; set => entityConvertor = value; }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <returns></returns>
        public string GetTableName<T>(string subTableArg = null)
        {
            string tableName = TableAttr?.Name;
            if (!string.IsNullOrEmpty(tableName) && tableName.Contains("{0}"))
            {
                ExHelper.ThrowIfNull(subTableArg, $"对象{typeof(T).Name},分表参数不能为空,请使用分表DbContext.调用GetSubTableDbContext获取分表DbContext");
                tableName = string.Format(tableName, subTableArg);
            }
            else if(string.IsNullOrEmpty(tableName))
            {   //未设置表名称.则以类名称作为表名称
                tableName = typeof(T).Name;
            }
            return tableName;
        }
    }
}
