using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Jc.Core.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using Jc.Core.Helper;
using System.Collections.Specialized;
using Jc.Core.Data.Model;
using Jc.Core.Data.Query;

namespace Jc.Core
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        #region SetMethods
        
        /// <summary>
        /// 保存or更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Set<T>(T dto, string tableNamePfx) where T : class, new()
        {
            return Set<T>(dto,null,tableNamePfx);
        }

        /// <summary>
        /// 保存or更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="select">查询属性</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Set<T>(T dto,Expression<Func<T, object>> select = null,string tableNamePfx = null) where T : class, new()
        {
            ExHelper.ThrowIfNull(dto, "操作对象不能为空.");
            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            object pkValue = dtoDbMapping.PkMap.Pi.GetValue(dto);
            if (pkValue == null)
            {
                rowCount = Add(dto, select, tableNamePfx);
            }
            else if (dtoDbMapping.PkMap.PropertyType == typeof(int) || dtoDbMapping.PkMap.PropertyType == typeof(int?))
            {   //自增Id int
                if ((int)pkValue == 0)
                {
                    rowCount = Add(dto, select, tableNamePfx);
                }
                else
                {
                    rowCount = Update(dto, select, tableNamePfx);
                }
            }
            else if (dtoDbMapping.PkMap.PropertyType == typeof(long) || dtoDbMapping.PkMap.PropertyType == typeof(long?))
            {   //自增Id long
                if ((long)pkValue == 0)
                {
                    rowCount = Add(dto, select, tableNamePfx);
                }
                else
                {
                    rowCount = Update(dto, select, tableNamePfx);
                }
            }
            else if (dtoDbMapping.PkMap.PropertyType == typeof(Guid) || dtoDbMapping.PkMap.PropertyType == typeof(Guid?))
            {   //Guid Id
                if ((Guid)pkValue == Guid.Empty)
                {
                    rowCount = Add(dto, select, tableNamePfx);
                }
                else
                {
                    rowCount = Update(dto, select, tableNamePfx);
                }
            }
            else
            {
                rowCount = Update(dto, select, tableNamePfx);
            }
            return rowCount;
        }
        
        /// <summary>
        /// 保存数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Add<T>(T dto, string tableNamePfx) where T : class, new()
        {
            return Add<T>(dto, null, tableNamePfx);
        }
        
        /// <summary>
        /// 保存数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="select">查询属性</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Add<T>(T dto, Expression<Func<T, object>> select = null, string tableNamePfx = null) where T : class, new()
        {
            ExHelper.ThrowIfNull(dto, "操作对象不能为空.");

            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            if (dtoDbMapping.TableAttr.AutoCreate)
            {   //如果是自动建表
                if (!CheckTableExists<T>())
                {
                    CreateTable<T>();
                }
            }
            object pkValue = dtoDbMapping.PkMap.Pi.GetValue(dto);
            if (dtoDbMapping.PkMap.PropertyType == typeof(int) || dtoDbMapping.PkMap.PropertyType == typeof(int?))
            {   //自增Id
            }
            else if (dtoDbMapping.PkMap.PropertyType == typeof(long) || dtoDbMapping.PkMap.PropertyType == typeof(long?))
            {
            }
            else if (dtoDbMapping.PkMap.PropertyType == typeof(Guid) || dtoDbMapping.PkMap.PropertyType == typeof(Guid?))
            {   //Guid Id
                if (pkValue == null || (Guid)pkValue == Guid.Empty)
                {   //生成Guid
                    dtoDbMapping.PkMap.Pi.SetValue(dto, Guid.NewGuid());
                }
            }

            using (DbCommand dbCommand = dbProvider.GetInsertDbCmd(dto, piMapList, tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    if (rowCount <= 0)
                    {
                        throw new Exception("插入记录失败.");
                    }
                    if ((dtoDbMapping.PkMap.PropertyType == typeof(int) 
                        || dtoDbMapping.PkMap.PropertyType == typeof(int?))
                        && (pkValue==null || (int)pkValue ==0))
                    {
                        int id = 0;
                        DbCommand getIdCommand = dbProvider.GetAutoIdDbCommand();
                        getIdCommand.Connection = dbCommand.Connection;
                        object valueObj = getIdCommand.ExecuteScalar();
                        if (valueObj != null && valueObj != DBNull.Value)
                        {
                            id = Convert.ToInt32(valueObj);
                        }
                        dtoDbMapping.PkMap.Pi.SetValue(dto, id);
                    }
                    else if ((dtoDbMapping.PkMap.PropertyType == typeof(long) || 
                        dtoDbMapping.PkMap.PropertyType == typeof(long?))
                        && (pkValue == null || (long)pkValue == 0))
                    {
                        long id = 0;
                        DbCommand getIdCommand = dbProvider.GetAutoIdDbCommand();
                        getIdCommand.Connection = dbCommand.Connection;
                        object valueObj = getIdCommand.ExecuteScalar();
                        if (valueObj != null && valueObj != DBNull.Value)
                        {
                            id = Convert.ToInt64(valueObj);
                        }
                        dtoDbMapping.PkMap.Pi.SetValue(dto, id);
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
        }
        
        /// <summary>
        /// 更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Update<T>(T dto, string tableNamePfx) where T : class, new()
        {
            return Update<T>(dto, null,tableNamePfx);
        }
        
        /// <summary>
        /// 更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="select">更新属性</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Update<T>(T dto,Expression<Func<T, object>> select = null,string tableNamePfx = null) where T : class, new()
        {
            ExHelper.ThrowIfNull(dto, "操作对象不能为空.");
            int rowCount = 0;
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            using (DbCommand dbCommand = dbProvider.GetUpdateDbCmd(dto, piMapList, tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    if (rowCount <= 0)
                    {
                        //throw new Exception("没有更新任何记录.");
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
        }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        public int DeleteById<T>(object id) where T : class, new()
        {
            return DeleteById<T>(id,null);
        }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int DeleteById<T>(object id, string tableNamePfx) where T : class, new()
        {
            int rowCount = 0;
            using (DbCommand dbCommand = dbProvider.GetDeleteByIdDbCmd<T>(id, tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    if (rowCount <= 0)
                    {
                        //throw new Exception("没有删除任何记录.");
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
        }


        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="dto">对象</param>
        public int Delete<T>(T dto) where T : class, new()
        {
            return Delete<T>(dto,null);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="dto">对象</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Delete<T>(T dto, string tableNamePfx) where T : class, new()
        {
            ExHelper.ThrowIfNull(dto, "操作对象不能为空.");

            int rowCount = 0;
            using (DbCommand dbCommand = dbProvider.GetDeleteDbCmd<T>(dto, tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    if (rowCount <= 0)
                    {
                        //throw new Exception("没有删除任何记录.");
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="where">删除条件</param>
        public int Delete<T>(Expression<Func<T, bool>> where) where T : class, new()
        {
            return Delete<T>(where,null);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="where">删除条件</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        public int Delete<T>(Expression<Func<T, bool>> where, string tableNamePfx) where T : class, new()
        {
            ExHelper.ThrowIfNull(where, "删除条件对象不能为空.");

            int rowCount = 0;
            QueryFilter filter = QueryFilterHelper.GetFilter(where);
            using (DbCommand dbCommand = dbProvider.GetDeleteDbCmd<T>(filter, tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    if (rowCount <= 0)
                    {
                        //throw new Exception("没有删除任何记录.");
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
        }
        
        /// <summary>
        /// 检查表是否已存在
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="tableNamePfx">
        /// 表名称,如果为空,则使用T对应表名称.
        /// 如果是可变表名称,请设置为表名称中的变量参数
        /// 如TableAttr中Name为Data{0}.此参数可传入2018.则表名称为Data2018
        /// </param>
        /// <returns></returns>
        public bool CheckTableExists<T>(string tableNamePfx = null) where T : class, new()
        {
            bool result = false;
            using (DbCommand dbCommand = dbProvider.GetCheckTableExistsDbCommand<T>(tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    DbDataReader dr = dbCommand.ExecuteReader();
                    result = dr.HasRows;
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 新建表
        /// </summary>
        /// <typeparam name="T">需要为T各属性指定相应的FieldDbType,及长度等属性</typeparam>
        /// <param name="tableNamePfx">
        /// 表名称,如果为空,则使用T对应表名称.
        /// 如果是可变表名称,请设置为表名称中的变量参数
        /// 如TableAttr中Name为Data{0}.此参数可传入2018.则表名称为Data2018
        /// </param>
        /// <returns></returns>
        public void CreateTable<T>(string tableNamePfx = null) where T : class, new()
        {
            using (DbCommand dbCommand = dbProvider.GetCreateTableDbCommand<T>(tableNamePfx))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    dbCommand.ExecuteNonQuery();
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
        }
        #endregion
    }
}