using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace Jc.Data.Query
{
    /// <summary>
    /// Represents a ORDER BY clause to be used with SELECT statements
    /// </summary>
    public struct OrderByClause
    {
        public string FieldName;
        public Sorting Order;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public OrderByClause(string field)
        {
            FieldName = field;
            Order = Sorting.Asc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="order"></param>
        public OrderByClause(string field, Sorting order)
        {
            FieldName = field;
            Order = order;
        }
    }
}
