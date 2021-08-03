
using System.Collections.Generic;
using System.Data;

namespace Jc.Core.FrameworkTest
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
        T ConvertDto(DataRow dr, EntityConvertResult convertResult);
    }
}
