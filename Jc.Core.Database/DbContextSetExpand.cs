using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using System.Collections.Specialized;
using Jc.Database.Data;
using Jc.Database.Query;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Jc.Database
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
        /// <param name="select">查询属性</param>
        public int Set<T>(T dto,Expression<Func<T, object>> select = null) where T : class, new()
        {
            if (dto == null)
            {
                throw new Exception("操作对象不能为空.");
            }
            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            object pkValue = dtoDbMapping.PkMap.Pi.GetValue(dto);
            
            if (CheckIsNullOrEmpty(pkValue, dtoDbMapping.PkMap.PropertyType))
            {
                rowCount = Add(dto, select);
            }
            else
            { 
                rowCount = Update(dto, select);
            }
            return rowCount;
        }

        /// <summary>
        /// 批量插入或批量更新记录,不能同时插入和更新数据
        /// 以第一条数据主键属性值判断,插入或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">保存进度</param>
        public int SetList<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            if (list == null || list.Count <= 0)
            {
                return 0;
            }
            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();

            object pkValue = dtoDbMapping.PkMap.Pi.GetValue(list[0]);
            if (CheckIsNullOrEmpty(pkValue, dtoDbMapping.PkMap.PropertyType))
            {
                rowCount = AddList(list, select, useTransaction, progress);
            }
            else
            {
                rowCount = UpdateList(list, select, useTransaction, progress);
            }
            return rowCount;
        }


        /// <summary>
        /// 批量保存数据
        /// 批量操作数据为插入和更新操作的数据List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">保存进度</param>
        private int SetList_Bak<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            if (list == null || list.Count <= 0)
            {
                return 0;
            }
            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<T> addList = new List<T>();
            List<T> updateList = new List<T>();

            #region 拆分新增或更新对象List
            for (int i = 0; i < list.Count; i++)
            {
                object pkValue = dtoDbMapping.PkMap.Pi.GetValue(list[i]);
                if (CheckIsNullOrEmpty(pkValue, dtoDbMapping.PkMap.PropertyType))
                {
                    addList.Add(list[i]);
                }
                else
                {
                    updateList.Add(list[i]);
                }
            }
            #endregion

            if (useTransaction)
            {
                if (addList.Count > 0 && updateList.Count > 0)
                {
                    throw new Exception("插入和更新操作同时存在时,不能启用事务.");
                }
            }
            if (addList.Count > 0)
            {
                rowCount += AddList(addList, select, useTransaction, progress);
            }
            if (updateList.Count > 0)
            {
                rowCount += UpdateList(updateList, select, useTransaction, progress);
            }
            return rowCount;
        }

        /// <summary>
        /// 异步批量保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象 List</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">进度通知</param>
        public async Task<int> SetListSync<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            int rowCount = await Task.Run(() => { return SetList(list, select, useTransaction, progress); });
            return rowCount;
        }
        
        /// <summary>
        /// 保存数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="select">查询属性</param>
        public int Add<T>(T dto, Expression<Func<T, object>> select = null) where T : class, new()
        {
            if (dto == null)
            {
                throw new Exception("待保存对象不能为空");
            }
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
            if (!dtoDbMapping.IsAutoIncrementPk)
            {   //Guid Id
                if (pkValue == null || (Guid)pkValue == Guid.Empty)
                {   //生成Guid
                    dtoDbMapping.PkMap.Pi.SetValue(dto, Guid.NewGuid());
                }
                if (!piMapList.Contains(dtoDbMapping.PkMap))
                {
                    piMapList.Insert(0, dtoDbMapping.PkMap);
                }
            }
            using (DbCommand dbCommand = dbProvider.GetInsertDbCmd(dto, piMapList, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 添加记录
                    if ((dtoDbMapping.PkMap.PropertyType == typeof(int)
                        || dtoDbMapping.PkMap.PropertyType == typeof(int?))
                        && (pkValue == null || (int)pkValue == 0))
                    {
                        dbCommand.CommandText += ";" + dbProvider.GetAutoIdDbCommand().CommandText;
                        object valueObj = DbCommandExecuter.ExecuteScalar(dbCommand);
                        if (valueObj == null || valueObj == DBNull.Value)
                        {
                            throw new Exception("插入记录失败.");
                        }
                        dtoDbMapping.PkMap.Pi.SetValue(dto, Convert.ToInt32(valueObj));
                    }
                    else if ((dtoDbMapping.PkMap.PropertyType == typeof(long) ||
                        dtoDbMapping.PkMap.PropertyType == typeof(long?))
                        && (pkValue == null || (long)pkValue == 0))
                    {
                        dbCommand.CommandText = $"{dbCommand.CommandText};{dbProvider.GetAutoIdDbCommand().CommandText}";
                        object valueObj = DbCommandExecuter.ExecuteScalar(dbCommand);
                        if (valueObj == null || valueObj == DBNull.Value)
                        {
                            throw new Exception("插入记录失败.");
                        }
                        dtoDbMapping.PkMap.Pi.SetValue(dto, Convert.ToInt64(valueObj));
                    }
                    else
                    {
                        rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
                        if (rowCount <= 0)
                        {
                            throw new Exception("插入记录失败.");
                        }
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
        /// 批量添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象 List</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">进度通知</param>
        public int AddList<T>(List<T> list, Expression<Func<T, object>> select = null,bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            if (list == null || list.Count <= 0)
            {
                return 0;
            }
            int rowCount = 0;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            if (!dtoDbMapping.IsAutoIncrementPk && !piMapList.Contains(dtoDbMapping.PkMap))
            {
                piMapList.Insert(0,dtoDbMapping.PkMap);
            }
            if (dtoDbMapping.TableAttr.AutoCreate)
            {   //如果是自动建表
                if (!CheckTableExists<T>())
                {
                    CreateTable<T>();
                }
            }
            //因为参数有2100的限制
            int perOpAmount = 2000/ piMapList.Count; //每次添加Amount

            if (useTransaction && !isTransaction)
            {
                #region Use Transaction
                DbConnection dbConnection = GetDbConnection();
                DbTransaction transaction = dbConnection.BeginTransaction();
                int i = 0;
                T curItem;
                try
                {
                    List<T> curOpList = new List<T>();
                    for (i = 0; i < list.Count; i++)
                    {
                        #region 设置Id
                        curItem = list[i];
                        object pkValue = dtoDbMapping.PkMap.Pi.GetValue(list[i]);
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
                                dtoDbMapping.PkMap.Pi.SetValue(list[i], Guid.NewGuid());
                            }
                        }
                        #endregion

                        curOpList.Add(list[i]);
                        if ((i + 1) % perOpAmount == 0 || i == list.Count - 1)
                        {
                            using (DbCommand dbCommand = dbProvider.GetInsertDbCmd(curOpList, piMapList, this.GetSubTableArg<T>()))
                            {
                                dbCommand.Connection = dbConnection;
                                dbCommand.Transaction = transaction;
                                rowCount += DbCommandExecuter.ExecuteNonQuery(dbCommand);
                            }
                            curOpList.Clear();
                            if (progress != null)
                            {
                                progress.Report((i + 1) * 1.0 / list.Count);
                            }
                        }
                    }
                    transaction.Commit();
                    CloseDbConnection(dbConnection);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    CloseDbConnection(dbConnection);
                    throw ex;
                }
                #endregion
            }
            else
            {
                #region Not Use Transaction
                int i = 0;
                T curItem;
                List<T> curOpList = new List<T>();
                for (i = 0; i < list.Count; i++)
                {
                    #region 设置Id
                    curItem = list[i];
                    object pkValue = dtoDbMapping.PkMap.Pi.GetValue(list[i]);
                    if (dtoDbMapping.PkMap.PropertyType == typeof(Guid) || dtoDbMapping.PkMap.PropertyType == typeof(Guid?))
                    {   //Guid Id
                        if (pkValue == null || (Guid)pkValue == Guid.Empty)
                        {   //生成Guid
                            dtoDbMapping.PkMap.Pi.SetValue(list[i], Guid.NewGuid());
                        }
                    }
                    #endregion

                    curOpList.Add(list[i]);
                    if ((i + 1) % perOpAmount == 0 || i == list.Count - 1)
                    {
                        using (DbCommand dbCommand = dbProvider.GetInsertDbCmd(curOpList, piMapList, this.GetSubTableArg<T>()))
                        {
                            try
                            {
                                SetDbConnection(dbCommand);
                                rowCount += DbCommandExecuter.ExecuteNonQuery(dbCommand);
                                CloseDbConnection(dbCommand);
                            }
                            catch (Exception ex)
                            { 
                                CloseDbConnection(dbCommand);
                                throw ex;
                            }
                        }
                        curOpList.Clear();
                        if (progress != null)
                        {
                            progress.Report((i + 1) * 1.0 / list.Count);
                        }
                    }
                }
                #endregion 
            }
            return rowCount;
        }

        /// <summary>
        /// 异步批量添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象 List</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">进度通知</param>
        public async Task<int> AddListSync<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            int rowCount = await Task.Run(() => { return AddList(list, select, useTransaction, progress); });
            return rowCount;
        }
        
        /// <summary>
        /// 更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="select">更新属性</param>
        public int Update<T>(T dto,Expression<Func<T, object>> select = null) where T : class, new()
        {
            if (dto == null)
            {
                throw new Exception("待更新对象不能为空.");
            }
            int rowCount = 0;

            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            using (DbCommand dbCommand = dbProvider.GetUpdateDbCmd(dto, piMapList, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 更新记录
                    rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
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
        /// 条件更新
        /// 如未包含主键=主键值,则会更新所有符合条件的记录
        /// 使用时,请确保条件正确,以免造成数据错误
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto">实体对象</param>
        /// <param name="where">更新条件</param>
        /// <param name="select">更新属性</param>
        public int Update<T>(T dto, Expression<Func<T, bool>> where ,Expression<Func<T, object>> select = null) where T : class, new()
        {
            if (dto == null)
            {
                throw new Exception("待更新对象不能为空.");
            }
            int rowCount = 0;

            QueryFilter filter = QueryFilterBuilder.GetFilter(where, select);

            using (DbCommand dbCommand = dbProvider.GetUpdateDbCmd(dto, filter, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 更新记录
                    rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
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
        /// 更新数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象</param>
        /// <param name="select">更新属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">进度通知</param>
        public int UpdateList<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            if (list == null || list.Count <= 0)
            {
                return 0;
            }
            int rowCount = 0;             
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            if (dtoDbMapping.IsAutoIncrementPk)
            {   //自增列Id不允许更新
                if (piMapList.Contains(dtoDbMapping.PkMap))
                {
                    piMapList.Remove(dtoDbMapping.PkMap);
                }
            }
            //因为参数有2100的限制 待更新字段 + 主键
            int perOpAmount = 2000 / (piMapList.Count + 1); //每次更新Amount

            if (useTransaction && !isTransaction)
            {
                #region Use Transaction
                DbConnection dbConnection = GetDbConnection();                
                DbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    List<T> curOpList = new List<T>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        curOpList.Add(list[i]);
                        if ((i + 1) % perOpAmount == 0 || i == list.Count - 1)
                        {
                            using (DbCommand dbCommand = dbProvider.GetUpdateDbCmd(curOpList, piMapList, this.GetSubTableArg<T>()))
                            {
                                dbCommand.Connection = dbConnection;
                                dbCommand.Transaction = transaction;
                                rowCount += DbCommandExecuter.ExecuteNonQuery(dbCommand);
                            }
                            curOpList.Clear();
                            if (progress != null)
                            {
                                progress.Report((i + 1) * 1.0 / list.Count);
                            }
                        }
                    }
                    transaction.Commit();
                    CloseDbConnection(dbConnection);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    CloseDbConnection(dbConnection);
                    throw ex;
                }                
                #endregion
            }
            else
            {
                #region Not Use Transaction
                List<T> curOpList = new List<T>();
                for (int i = 0; i < list.Count; i++)
                {
                    curOpList.Add(list[i]);
                    if ((i + 1) % perOpAmount == 0 || i == list.Count - 1)
                    {
                        using (DbCommand dbCommand = dbProvider.GetUpdateDbCmd(curOpList, piMapList, this.GetSubTableArg<T>()))
                        {
                            try
                            {
                                SetDbConnection(dbCommand);
                                rowCount += DbCommandExecuter.ExecuteNonQuery(dbCommand);
                            }
                            catch (Exception ex)
                            {
                                CloseDbConnection(dbCommand);
                                throw ex;
                            }
                        }
                        curOpList.Clear();
                        if (progress != null)
                        {
                            progress.Report((i + 1) * 1.0 / list.Count);
                        }
                    }
                }
                #endregion 
            }
            return rowCount;
        }

        /// <summary>
        /// 异步批量更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">实体对象 List</param>
        /// <param name="select">查询属性</param>
        /// <param name="useTransaction">使用事务操作 默认true</param>
        /// <param name="progress">进度通知</param>
        public async Task<int> UpdateListSync<T>(List<T> list, Expression<Func<T, object>> select = null, bool useTransaction = true, IProgress<double> progress = null) where T : class, new()
        {
            int rowCount = await Task.Run(() => { return UpdateList(list, select, useTransaction, progress); });
            return rowCount;
        }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id"></param>
        public int DeleteById<T>(object id) where T : class, new()
        {
            int rowCount = 0;
            using (DbCommand dbCommand = dbProvider.GetDeleteByIdDbCmd<T>(id, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 删除记录
                    rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
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
            if (dto == null)
            {
                throw new Exception("待删除对象不能为空");
            }
            int rowCount = 0;
            using (DbCommand dbCommand = dbProvider.GetDeleteDbCmd<T>(dto, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 删除记录
                    rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
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
            if (where == null)
            {
                throw new Exception("删除条件不能为空");
            }
            int rowCount = 0;
            QueryFilter filter = QueryFilterBuilder.GetFilter(where);
            using (DbCommand dbCommand = dbProvider.GetDeleteDbCmd<T>(filter, this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 删除记录
                    rowCount = DbCommandExecuter.ExecuteNonQuery(dbCommand);
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
        /// <returns></returns>
        public bool CheckTableExists<T>() where T : class, new()
        {
            bool result = false;
            string subTableArg = this.GetSubTableArg<T>();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.GetTableName(subTableArg);

            using (DbCommand dbCommand = dbProvider.GetCheckTableExistsDbCommand(tableName))
            {
                try
                {
                    SetDbConnection(dbCommand);
                    using (DbDataReader dr = DbCommandExecuter.ExecuteReader(dbCommand))
                    {
                        result = dr.HasRows;
                        dr.Close();
                    }
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
        /// <returns></returns>
        public void CreateTable<T>() where T : class, new()
        {
            using (DbCommand dbCommand = dbProvider.GetCreateTableDbCommand<T>(this.GetSubTableArg<T>()))
            {
                try
                {
                    SetDbConnection(dbCommand); // 创建表
                    DbCommandExecuter.ExecuteNonQuery(dbCommand);
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Obj IsNullOrEmpty
        /// 默认严格模式下string.Empty,Guid.Empty,int 0,ICollection.Count 0 都会被作为false
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="objType">对象类型</param>
        /// <param name="isStrict">严格模式</param>
        /// <returns></returns>
        private bool CheckIsNullOrEmpty(object obj,Type objType, bool isStrict = true)
        {
            bool result = false;
            if (obj == null)
            {
                result = true;
            }
            else if (isStrict)
            {
                Type type = objType;
                if (type == typeof(string))
                {
                    if (string.IsNullOrEmpty(obj.ToString()))
                    {
                        result = true;
                    }
                }
                else if (type == typeof(Guid) || type == typeof(Guid?))
                {
                    if (((Guid)obj) == Guid.Empty)
                    {
                        result = true;
                    }
                }
                else if (type == typeof(int) || type == typeof(int?))
                {
                    if (((int)obj) == 0)
                    {
                        result = true;
                    }
                }
                else if (type == typeof(long) || type == typeof(long?))
                {
                    if (((long)obj) == 0)
                    {
                        result = true;
                    }
                }
                else if (obj is ICollection)
                {
                    ICollection collection = obj as ICollection;
                    if (collection.Count <= 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 使用BulkCopy插入数据
        /// 目前仅支持MsSql(Sql Server)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数据List</param>
        /// <param name="batchSize">BatchSize</param>
        /// <param name="timeout">BulkCopyTimeout</param>
        /// <param name="useTransaction">使用事务</param>
        /// <param name="progress">0,1 进度</param>
        public void BulkCopy<T>(List<T> list, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null) where T : class, new()
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.GetTableName();
            DataTable dt = EntityToDataTable(list);
            dbProvider.BulkCopy(tableName, dt, batchSize, timeout, useTransaction, progress);
        }

        /// <summary>
        /// Convert List To DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private DataTable EntityToDataTable<T>(List<T> list) where T : class, new()
        {
            DataTable dt = GetDataTable<T>(" 1 = 0");
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.GetTableName();
            List<PiMap> piMaps = DtoMappingHelper.GetPiMapList<T>();

            Dictionary<DataColumn, PiMap> dtPiDic = new Dictionary<DataColumn, PiMap>();

            List<DataColumn> columns = dt.Columns.Cast<DataColumn>().ToList();
            for (int i = 0; i < columns.Count; i++)
            {
                DataColumn column = columns[i];
                PiMap piMap = piMaps.FirstOrDefault(a => a.FieldName.ToLower() == column.ColumnName.ToLower());
                if (piMap != null)
                {
                    if (piMap.PropertyType != column.DataType)
                    {
                        Type realType = piMap.PropertyType.GenericTypeArguments.Length > 0 ? piMap.PropertyType.GenericTypeArguments[0] : piMap.PropertyType;
                        if (realType.IsEnum)
                        {
                            realType = typeof(int);
                        }
                        if (realType != column.DataType)
                        {
                        }
                    }
                    dtPiDic.Add(column, piMap);
                }
                else
                { 
                }
            }

            dt.Clear();
            foreach (T dto in list)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn column in columns)
                {
                    object value = null;
                    if (dtPiDic.ContainsKey(column))
                    {
                        PiMap piMap = dtPiDic[column];

                        value = piMap.Pi.GetValue(dto);

                        if (piMap == dtoDbMapping.PkMap && !dtoDbMapping.IsAutoIncrementPk)
                        {   //是否需要生成Guid
                            if (value == null || (Guid)value == Guid.Empty)
                            {
                                value = Guid.NewGuid();
                                piMap.Pi.SetValue(dto, value);
                            }
                        }
                        if (piMap.IsEnum)
                        {
                            if (value != null)
                                value = (int)value;
                        }
                    }
                    dr[column] = value ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion
    }
}