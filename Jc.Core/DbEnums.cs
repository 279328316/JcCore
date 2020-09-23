using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jc.Core
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Ms SqlServer
        /// </summary>
        MsSql = 1,
        /// <summary>
        /// Sqlite 使用Guid时,请在连接串中添加BinaryGUID=False;
        /// connectionString = "Data Source=|DataDirectory|\data\Db.db3;password=abcd;BinaryGUID=False;Pooling=true;FailIfMissing=true;" providerName = "System.Data.SQLite"
        /// </summary>
        Sqlite = 2,
        /// <summary>
        /// MySql
        /// </summary>
        MySql = 3,
        /// <summary>
        /// PostgreSql
        /// </summary>
        PostgreSql = 4,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 5
    }

    /// <summary>
    /// 排序方向
    /// </summary>
    public enum Sorting
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }

    /// <summary>
    /// 表类型
    /// </summary>
    public enum TableType
    {
        /// <summary>
        /// 表
        /// </summary>
        Table = 1,
        /// <summary>
        /// 视图
        /// </summary>
        View = 2
    }

    /// <summary>
    /// 连接运算
    /// </summary>
    public enum Conjuction
    {
        /// <summary>
        /// And
        /// </summary>
        And = 1,
        /// <summary>
        /// Or
        /// </summary>
        Or = 2,
        /// <summary>
        /// Where
        /// </summary>
        Where = 3,
        /// <summary>
        /// None
        /// </summary>
        None = 4
    }

    /// <summary>
    /// 操作运算
    /// </summary>
    public enum Operand
    {
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
        Like = 7,
        LeftLike = 8,
        RightLike = 9,
        IsNull = 10,
        IsNotNull = 11,
        /// <summary>
        /// 使用逗号分隔.样例"1,2,3","东,西,南,北"
        /// </summary>
        In = 12,
        /// <summary>
        /// 使用逗号分隔.样例"1,2,3","东,西,南,北"
        /// </summary>
        NotIn = 13,
        NotLike = 14,

        /// <summary>
        /// Field字段Equal
        /// </summary>
        FieldEqual = 15,
        FieldNotEqual = 16,
        FieldLessThan = 17,
        FieldLessThanOrEqual = 18,
        FieldGreaterThan = 19,
        FieldGreaterThanOrEqual = 20,
        None = 21,
        Contains = 22
    }
}
