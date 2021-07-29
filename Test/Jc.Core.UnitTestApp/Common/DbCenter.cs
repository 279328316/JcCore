﻿using Jc.Core;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Jc.Core.UnitTestApp
{
    /// <summary>
    /// DbCenter
    /// </summary>
    public class Dbc
    {
        /// <summary>
        /// Db
        /// </summary>
        public static DbContext Db = DbContext.CreateDbContext("Stock", DatabaseType.MsSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext NiceDb = DbContext.CreateDbContext("Nice", DatabaseType.MsSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext StockDb = DbContext.CreateDbContext("Stock", DatabaseType.MsSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext PetDb = DbContext.CreateDbContext("PETCT_PUMC", DatabaseType.PostgreSql);

        /// <summary>
        /// 获取DbIQuery
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQuery<T> GetIQuery<T>(IEnumerable<KeyValuePair<string, StringValues>> collection) where T : class, new()
        {
            return Db.IQuery<T>(collection);
        }
    }
}
