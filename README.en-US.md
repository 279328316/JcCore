# JcCore

JcCore is a simple and easy-to-use open-source .NET ORM framework.

Supports SqlServer, MySql, Sqlite, and Postgresql databases.

Simple usage:

1. Create a `DbCenter.cs` file:

    /// <summary>
    /// DbCenter
    /// </summary>
    public class Dbc
    {
        /// <summary>
        /// Db
        /// </summary>
        public static DbContext Db = DbContext.CreateDbContext("Nice", DatabaseType.MsSql);
    }

Then you can use `Dbc.Db` throughout your project.

2. Use `Dbc.Db` to perform basic CRUD operations.


# Team Services

1. Basic usage consultation and bug report fixes.

2. Submit suggestions and requirements via the QQ exchange group: 744340299.

# Declaration

You may use, modify, and redistribute this software freely without notification.

Published interfaces may be subject to change; please pay attention to compatibility issues whenever updating versions.

If you find a bug, please open an issue or provide feedback in the QQ group.
