using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using System.Runtime.Serialization;



namespace Jc.Database.Query
{
    public class QueryParameter
    {
        #region Fields
        private string parameterName;
        private DbType parameterDbType = DbType.String;
        private object parameterValue;

        private string fieldName;
        private Operand op= Operand.Equal;
        private Conjuction conj = Conjuction.And;
        #endregion

        #region Properties
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName
        {
            get
            {
                return parameterName;
            }
            set
            {
                parameterName = value;
            }
        }
        /// <summary>
        /// 参数类型
        /// </summary>
        public DbType ParameterDbType
        {
            get
            {
                return parameterDbType;
            }
            set
            {
                parameterDbType = value;
            }
        }
        /// <summary>
        /// 参数值
        /// </summary>
        public object  ParameterValue
        {
            get
            {
                return parameterValue;
            }
            set
            {
                parameterValue = value;
            }
        }
        /// <summary>
        /// 对应字段名称
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }
        /// <summary>
        /// 操作符号
        /// </summary>
        public Operand Op
        {
            get
            {
                return op;
            }
            set
            {
                op = value;
            }
        }
        /// <summary>
        /// 链接符号
        /// </summary>
        public Conjuction Conj
        {
            get
            {
                return conj;
            }
            set
            {
                conj = value;
            }
        }
        #endregion
    }
}
