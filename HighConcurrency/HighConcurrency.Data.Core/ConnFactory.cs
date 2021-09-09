using HighConcurrency.Data.Core.Config;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HighConcurrency.Data.Core
{
    ///*********************************在Dapper 底层的基础上进行封装一次****************************///

    /// <summary>
    /// 后期可以手动添加MySql，Sqlite等
    /// </summary>
    public partial class ConnFactory
    {
        private static readonly string connString = "server=47.119.154.135;uid=admin;pwd=admin123;port=3306;database=localservicetest;";//sslmode=Preferred;Allow User Variables=True;
        /// <summary>
        /// 获取Connection
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class DapperRepository
    {

        #region
        /// <summary>
        /// 事务开启
        /// </summary>
        /// <returns></returns>
        public async Task<IDbTransaction> BeginTransaction()
        {
            var conn = ConnFactory.GetConnection();
            await conn.OpenAsync();
            return await DapperDataAsync.BeginTransactionData(conn);
        }
        /// <summary>
        /// 事务封装
        /// </summary>
        /// <typeparam name="TA"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<TA> BeginTransactionFunction<TA>(Func<IDbTransaction, TA> func)
        {
            TA res = default;
            using (IDbTransaction conn = await this.BeginTransaction())
            {
                try
                {
                    if (func != null)
                    {
                        res = func.Invoke(conn);
                    }
                    var t = (res as Task);
                    t?.Wait();
                    conn.Commit();
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    throw ex;
                }
                finally
                {
                    conn.Connection.Close();
                }
            }
            return res;
        }
        #endregion

        /// <summary>
        /// sql 操作返回受影响的id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            //有事务
            if (transaction != null)
            {
                return await DapperDataAsync.ExecuteScalarAsync<T>((MySqlConnection)transaction.Connection, sql, param, transaction, commandTimeout, commandType);
            }
            //无事务
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.ExecuteScalarAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }

        #region 查询系列
        /// <summary>
        /// 强类型查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.QueryAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }

        /// <summary>
        //动态类型查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> QueryAsyncDynamic(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.QueryAsync(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }

        public async Task<Tuple<int, IEnumerable<T>>> PageLoadAsync<T>(string sql, object param = null, int pageIndex = 1, int pageSize = 20, string sortBySql = "")
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.PageLoadAsync<T>(conn, sql, param, pageIndex, pageSize, sortBySql);
            }
        }
        #endregion

        #region sql返回受影响的行数
        public async Task<int> ExecuteAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction != null)
            {
                return await DapperDataAsync.ExecuteAsync((MySqlConnection)transaction.Connection, sql, param, transaction, commandTimeout, commandType);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.ExecuteAsync(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }
        #endregion


        #region 扩展方法

        #region 扩展方法查询系列
        /// <summary>
        /// 获取Model-Key为 int 类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 获取Model-Key为long类型
        /// </summary>
        /// <typeparam name=""></typeparam>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(long id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 获取Model-Key为Guid类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(System.Guid id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 获取Model-Key为String类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <returns></returns>
        //public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        //{
        //    using (var conn = ConnFactory.GetConnection())
        //    {
        //        await conn.OpenAsync();
        //        return await DapperDataAsync.GetAllAsync<T>(conn);
        //    }
        //}
        #endregion

        #region 扩展方法增删改
        /// <summary>
        /// 新增方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (transaction != null)
            {
                return await DapperDataAsync.InsertAsync<TEntity>((MySqlConnection)transaction.Connection, entity, transaction, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.InsertAsync<TEntity>(conn, entity, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 异步的修改方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (transaction != null)
            {
                return await DapperDataAsync.UpdateAsync<TEntity>((MySqlConnection)transaction.Connection, entity, transaction, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.UpdateAsync<TEntity>(conn, entity, transaction, commandTimeout);
            }
        }


        /// <summary>
        /// 同步的修改方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public bool Update<TEntity>(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (transaction != null)
            {
                return DapperDataAsync.Update<TEntity>((MySqlConnection)transaction.Connection, entity, transaction, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                conn.OpenAsync();
                return DapperDataAsync.Update<TEntity>(conn, entity, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<TEntity>(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (transaction != null)
            {
                return await DapperDataAsync.DeleteAsync<TEntity>((MySqlConnection)transaction.Connection, entity, transaction, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.DeleteAsync<TEntity>(conn, entity, transaction, commandTimeout);
            }
        }
        #endregion


        #endregion

    }
}