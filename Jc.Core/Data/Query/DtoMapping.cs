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
            get { return TableAttr.Name; }
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
                    if (pkFieldPair.Key == null)
                    {
                        pkFieldPair = piMapDic.Where(map => map.Key.ToLower() == "id").FirstOrDefault();
                    }
                    if (pkFieldPair.Key == null)
                    {
                        throw new Exception("未为表" + TableName + "设置主键.");
                    }
                    pkMap = piMapDic[pkFieldPair.Key];
                    if (pkMap == null)
                    {
                        throw new Exception("未为表" + TableName + "设置主键.");
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
        /// EntityConvertorDelegate 实际值为 EntityConvertorDelegate<T>
        /// 使用时,再设置
        /// </summary>
        public object EntityConvertor { get => entityConvertor; set => entityConvertor = value; }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <returns></returns>
        public string GetTableName<T>(string tableNamePfx = null)
        {
            string tableName = TableAttr.Name;
            if (TableAttr.Variable)
            {
                ExHelper.ThrowIfNull(tableNamePfx, $"可变表名称,对象{typeof(T).Name}传入动态参数不能为空.");
                if (string.IsNullOrEmpty(tableName) && !tableName.Contains("{0}"))
                {   //如果表名称为空或未设置填充参数{0},则直接使用传入参数tableNamePfx作为表名
                    tableName = tableNamePfx;
                }
                else
                {
                    tableName = string.Format(tableName, tableNamePfx);
                }
            }
            else
            {   //如果不可变表名称,传入tableNamePfx,则使用用户定义表名称
                if (!string.IsNullOrEmpty(tableNamePfx))
                {
                    tableName = tableNamePfx;
                }
            }
            return tableName;
        }
    }
}
