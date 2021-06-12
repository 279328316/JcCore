using System;
using System.Linq.Expressions;

namespace Jc.Excel
{
    /// <summary>
    /// Field Map
    /// </summary>
    public class FieldMap
    {
        #region Properties
        /// <summary>
        /// Id 标识
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 是否为序号列
        /// </summary>
        internal bool IsRowIndex { get; set; }

        /// <summary>
        /// 是否选中,仅作为标识字段使用
        /// </summary>
        public bool? IsSelected { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldText { get; set; }
        
        /// <summary>
        /// 列Index
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 属性名称
        /// PiName为RowIndex列为预留序号列
        /// </summary>
        public string PiName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// 字段格式
        /// </summary>
        public string FieldFormat { get; set; }

        /// <summary>
        /// 超链接属性名
        /// </summary>
        public string HyperLink { get; set; }
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public FieldMap()
        {
        }


        /// <summary>
        /// Ctor
        /// </summary>
        public FieldMap(string piName, string fieldText, FieldType type = FieldType.String, string format = null,string hyperLink = null)
        {
            this.FieldText = fieldText;
            this.PiName = piName;
            this.FieldType = type;
            this.FieldFormat = format;
            this.HyperLink = hyperLink;
        }

        /// <summary>
        /// 选中标识
        /// </summary>
        /// <returns></returns>
        public FieldMap Select()
        {
            this.IsSelected = true;
            return this;
        }
        #endregion
    }

    /// <summary>
    /// 字段序号列Map
    /// </summary>
    public class FieldRowIndexMap : FieldMap
    {
        /// <summary>
        /// 字段序号列Map
        /// </summary>
        public FieldRowIndexMap(string fieldText = "序号")
        {
            IsRowIndex = true;
            PiName = "RowIndex";
            FieldText = fieldText;
        }
    }

    /// <summary>
    /// Field Map
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FieldMap<T> : FieldMap
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public FieldMap(Expression<Func<T, object>> exp, string fieldText, Expression<Func<T, object>> hyperLinkExp)
        {
            this.FieldText = fieldText;
            this.PiName = GetPropertyName(exp);
            this.HyperLink = hyperLinkExp != null ? GetPropertyName(hyperLinkExp) : null;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public FieldMap(Expression<Func<T, object>> exp, string fieldText, FieldType type = FieldType.String, string format = null, Expression<Func<T, object>> hyperLinkExp = null)
        {
            this.FieldText = fieldText;
            this.PiName = GetPropertyName(exp);
            this.FieldType = type;
            this.FieldFormat = format;
            this.HyperLink = hyperLinkExp != null ? GetPropertyName(hyperLinkExp) : null;
        }


        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private string GetPropertyName(Expression<Func<T,object>> exp)
        {
            if (exp == null)
            { 
                throw new Exception($"Expression 不能为空");
            }
            string piName = "";
            if (exp.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression unaryExpression = exp.Body as UnaryExpression;
                MemberExpression memberExp = unaryExpression.Operand as MemberExpression;
                piName = memberExp.Member.Name;
            }
            else if (exp.Body is MemberExpression)
            {
                MemberExpression memberExp = exp.Body as MemberExpression;
                piName = memberExp.Member.Name;
            }
            else
            {
                throw new Exception($"不支持的 Expression {exp.ToString()}");
            }
            return piName;
        }
    }

}
