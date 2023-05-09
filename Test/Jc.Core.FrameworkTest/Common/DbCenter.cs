using Jc;
using Jc.Database;
using System.Collections.Generic;

namespace Jc.Core.FrameworkTest
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

    }
}
