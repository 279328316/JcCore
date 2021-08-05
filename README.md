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

## 团队服务

1.基本用法咨询
2.反馈bug修复
3.提交建议和需求
4.qq交流群讨论 744340299  qq 279328316

## 免责申明

- 虽然我们对代码已经进行高度审查,但依然可能存在某些未知的bug.

- 我们可能会对已发布的接口做修改，每当更新代码时，请注意兼容性问题。

## License

> 你可以在任意场景下使用JcCore Orm框架

> 时间之海，唯真善美永恒，唯有爱最珍贵。
> 繁星点点，人生经历万千，愿你穿越沧桑，仍是一个有所坚持的人。

