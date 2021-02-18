using Jc.Core;
using System.Collections.Generic;

namespace Jc.Core.TestApp
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
        /// PetCtDb
        /// </summary>
        public static DbContext PetCtDb = DbContext.CreateDbContext("PETCT_PUMC", DatabaseType.PostgreSql); //PETCT_PUMC Db 数据操作

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext YamDb = DbContext.CreateDbContext("YamCloud", DatabaseType.MySql);

    }
}
