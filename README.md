# JcCore
JcCore 简单好用的.net开源Orm框架

支持SqlServer,MySql,Sqlite,Postgresql数据库

使用方法简单:

1.创建一个DbCenter.cs文件

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

然后就可以在项目中使用Dbc.Db了.
2.使用Dbc.Db进行增删改查基本操作
