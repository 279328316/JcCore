using Jc.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jc.Database.Query
{
    public static class DataSetExpand
    {
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            List<T> list = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                EntityMapping dtoMapping = EntityMappingHelper.GetMapping<T>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr.ToEntity<T>());
                }
            }
            return list;
        }

        /// <summary>
        /// dr转换为Dto实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dr) where T : class, new()
        {
            EntityMapping dtoMapping = EntityMappingHelper.GetMapping<T>();
            if (dtoMapping.EntityConvertor == null)
            {
                dtoMapping.EntityConvertor = EntityConvertor.CreateEntityConvertor<T>();   //存入dtoMapping中,缓存起来
            }
            EntityConvertResult convertResult = new EntityConvertResult();
            T dto = (T)((EntityConvertorDelegate)dtoMapping.EntityConvertor)(dr, convertResult);
            if (convertResult.IsException)
            {
                throw new Exception($"load column '{convertResult.ColumnName}' error :{convertResult.Message}");
            }
            return dto;
        }
    }
}
