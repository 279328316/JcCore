using Jc;
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
        public static DbContext Db = DbContext.CreateDbContext("Test", DatabaseType.MsSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext NiceDb = DbContext.CreateDbContext("Nice", DatabaseType.MsSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext StockDb = DbContext.CreateDbContext("Stock", DatabaseType.MsSql);

    }
}
