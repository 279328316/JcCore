using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Database.Query
{
    /// <summary>
    /// 对象 Entity Mapping To Table
    /// </summary>
    public class EntityMapping
    {
        private Type entityType;  //T Type
        private TableAttribute tableAttr;   //Table Attr
        private string primaryTableName;
        private readonly Dictionary<string, FieldMapping> fieldMappings = new Dictionary<string, FieldMapping>();    //piMapDic
        private FieldMapping pkField; //pkMap
        private bool isAutoIncrementPk = false;
        private object entityConvertor;    //实际值为 EntityConvertorDelegate<T>

        #region Properties

        /// <summary>
        /// 表属性
        /// </summary>
        public TableAttribute TableAttr { get => tableAttr; }

        /// <summary>
        /// EntityType
        /// </summary>
        public Type EntityType { get => entityType; }

        /// <summary>
        /// 表对象名称
        /// </summary>
        public string PrimaryTableName { get => primaryTableName; }

        /// <summary>
        /// 类属性名与属性,字段关系映射列表
        /// </summary>
        public Dictionary<string, FieldMapping> FieldMappings { get => fieldMappings; }

        /// <summary>
        /// 主键字段映射Map
        /// </summary>
        public FieldMapping PkField { get => pkField; }

        /// <summary>
        /// 是否为自增主键
        /// </summary>
        public bool IsAutoIncrementPk { get => isAutoIncrementPk; }

        /// <summary>
        /// EntityConvertorDelegate 实际值为 EntityConvertorDelegate(T)
        /// 使用时,再设置
        /// </summary>
        public object EntityConvertor { get => entityConvertor; set => entityConvertor = value; }

        #endregion

        #region  Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public EntityMapping(Type entityType)
        {
            this.entityType = entityType;
            this.primaryTableName = TableAttribute.GetTableName(entityType);

            this.tableAttr = Attribute.GetCustomAttribute(entityType, typeof(TableAttribute)) as TableAttribute;

            InitFieldMapping(entityType, tableAttr);
        }

        private void InitFieldMapping(Type type,TableAttribute tableAttr)
        {
            PropertyInfo[] piList = type.GetProperties();
            foreach (PropertyInfo pi in piList)
            {
                FieldMapping fieldMap = new FieldMapping(pi, tableAttr);
                this.fieldMappings.Add(fieldMap.PiName, fieldMap);
            }
            this.pkField = GetTablePkField(fieldMappings);
            this.isAutoIncrementPk = CheckIsAutoIncreament(pkField);
        }
        #endregion

        /// <summary>
        /// 获得指定成员的特性对象
        /// </summary>
        /// <typeparam name="T">要获取属性的类型</typeparam>
        /// <param name="pInfo">属性原型</param>
        /// <returns>返回T对象</returns>
        private static T GetCustomAttribute<T>(PropertyInfo pInfo) where T : Attribute, new()
        {
            Type attributeType = typeof(T);
            Attribute attrObj = Attribute.GetCustomAttribute(pInfo, attributeType);
            T rAttrObj = attrObj as T;
            return rAttrObj;
        }

        /// <summary>
        /// 获取PkField
        /// </summary>
        /// <returns></returns>
        private FieldMapping GetTablePkField(Dictionary<string, FieldMapping> fieldMappings)
        {
            FieldMapping mapping = null;
            KeyValuePair<string, FieldMapping> pkFieldPair = fieldMappings.Where(map => map.Value.FieldAttribute.IsPk == true).FirstOrDefault();
            if (pkFieldPair.Key != null)
            {
                mapping = fieldMappings[pkFieldPair.Key];
            }
            else
            {
                string pkFiledName = !string.IsNullOrEmpty(TableAttr?.PkField) ? TableAttr.PkField : "Id";

                pkFieldPair = fieldMappings.Where(map => map.Key.ToLower() == pkFiledName.ToLower()).FirstOrDefault();
                if (pkFieldPair.Key != null)
                {
                    mapping = fieldMappings[pkFieldPair.Key];
                    mapping.FieldAttribute.IsPk = true;
                }
                else
                {
                    throw new Exception($"请为 Class {EntityType.Name} 设置主键字段");
                }
            }
            return mapping;
        }

        /// <summary>
        /// 检查是否为自增字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool CheckIsAutoIncreament(FieldMapping field)
        {
            bool result = false;
            if (field != null)
            {
                if (field.PropertyType == typeof(int) || field.PropertyType == typeof(int?))
                {   //自增Id
                    result = true;
                }
                else if (field.PropertyType == typeof(long) || field.PropertyType == typeof(long?))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <returns></returns>
        public string GetTableName(object subTableArg = null)
        {
            string targetTableName = primaryTableName;
            if (targetTableName.Contains("{0}"))
            {   // 处理分表情况
                if (subTableArg == null)
                {
                    throw new Exception($"对象{EntityType.Name},分表参数不能为空,请使用分表DbContext.调用GetSubTableDbContext获取分表DbContext");
                }
                targetTableName = string.Format(targetTableName, subTableArg.ToString().ToLower());
            }
            return targetTableName;
        }
    }
}
