using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jc.Core.Data.Query
{
    /// <summary>
    /// IEntityConvertor
    /// </summary>
    /// <typeparam name="T">Dto</typeparam>
    public interface IEntityConvertor<T>
    {
        /// <summary>
        /// 转换DataRow为Dto
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        T ConvertDto(DataRow dr);
    }
}
