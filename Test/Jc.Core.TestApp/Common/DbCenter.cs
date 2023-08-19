using Jc;
using Jc.Database;

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
        /// PgNiceDb
        /// </summary>
        public static DbContext PgTestDb = DbContext.CreateDbContext("PgTestDb", DatabaseType.PostgreSql);

        /// <summary>
        /// Db
        /// </summary>
        public static DbContext StockDb = DbContext.CreateDbContext("Stock", DatabaseType.MsSql);
    }
}
