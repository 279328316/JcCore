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
        /// ��������
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
        /// ��������
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
        /// ����ֵ
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
        /// ��Ӧ�ֶ�����
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
        /// ��������
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
        /// ���ӷ���
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
