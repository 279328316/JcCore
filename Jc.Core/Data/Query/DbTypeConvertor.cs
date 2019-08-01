using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jc.Core.Data.Query
{
    public static class DbTypeConvertor
    {
        // C#数据类型转换为DbType
        public static DbType TypeToDbType(Type t)
        {
            DbType dbt;
            try
            {
                string typeName = t.Name;
                if(t.GenericTypeArguments!=null && t.GenericTypeArguments.Length>0)
                {
                    typeName = t.GenericTypeArguments[0].Name;
                }
                dbt = (DbType)Enum.Parse(typeof(DbType), typeName);
            }
            catch
            {
                dbt = DbType.Object;
            }
            return dbt;
        }

        // C#数据类型转换为DbType
        public static DbType GetDbType(object value)
        {
            DbType dbt = DbType.Object;
            try
            {
                if (value != null)
                {
                    Type t = value.GetType();
                    string typeName = t.Name;
                    if (t.GenericTypeArguments != null && t.GenericTypeArguments.Length > 0)
                    {
                        typeName = t.GenericTypeArguments[0].Name;
                    }
                    dbt = (DbType)Enum.Parse(typeof(DbType), typeName);
                }
            }
            catch
            {
            }
            return dbt;
        }
    }
}
