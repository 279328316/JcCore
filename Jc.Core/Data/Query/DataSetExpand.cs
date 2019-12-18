using Jc.Core.Data.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jc.Core.Data
{
    public static class DataSetExpand
    {
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            List<T> list = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                DtoMapping dtoMapping = DtoMappingHelper.GetDtoMapping<T>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr.ToEntity<T>());
                }
                dt.Dispose();
                dt = null;
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
            DtoMapping dtoMapping = DtoMappingHelper.GetDtoMapping<T>();
            if (dtoMapping.EntityConvertor == null)
            {
                dtoMapping.EntityConvertor = EntityConvertor.CreateEntityConvertor<T>();   //存入dtoMapping中,缓存起来
            }
            return (T)((EntityConvertorDelegate)dtoMapping.EntityConvertor)(dr);
        }
    }
}
