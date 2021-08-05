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


# 团队服务

1.基本用法咨询,反馈bug修复

2.提交建议和需求   qq交流群讨论 744340299         qq 279328316

# 申明

可以随意使用,修改,再发行而不需要告知

可能会对已发布的接口做修改,每当更新版本时,请注意兼容性问题

如发现bug,请提issue或在qq群中反馈
