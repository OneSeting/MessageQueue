using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
//using DapperExtensions;
using MySql.Data.MySqlClient;
using System;

namespace HighConcurrency.Data.Core
{

    ///*********************************在Dapper 底层的拓展方法****************************///
    ///
    /// <summary>
    /// 扩展方法
    /// </summary>
    public abstract partial class DapperDataAsync
    {
        public static async Task<IDbTransaction> BeginTransactionData(MySqlConnection mySqlConnection)
        {
            return await mySqlConnection.BeginTransactionAsync();
        }

        #region 查询系
        /// <summary>
        /// 获取Model-Key为int类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection conn, int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为long类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection conn, long id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为Guid类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection conn, System.Guid id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为string类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection conn, string id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //public static async Task<IEnumerable<T>> GetAllAsync<T>(MySqlConnection conn) where T : class, new()
        //{
        //    return await conn.GetListAsync<T>();
        //}

        #endregion

        #region 增删改
        /// <summary>
        /// 插入一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static async Task<int> InsertAsync<T>(MySqlConnection conn, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.InsertAsync<T>(model, transaction, commandTimeout);
        }

        /// <summary>
        /// 异步更新一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateAsync<T>(MySqlConnection conn, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.UpdateAsync<T>(model, transaction, commandTimeout);
            //if (b) { return model; }
            //else { return null; }
        }

        /// <summary>
        /// 同步更新一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Update<T>(MySqlConnection conn, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return conn.Update<T>(model, transaction, commandTimeout);
            //if (b) { return model; }
            //else { return null; }
        }

        /// <summary>
        /// 删除一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteAsync<T>(MySqlConnection conn, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await conn.DeleteAsync<T>(model, transaction, commandTimeout);
            //if (b) { return model; }
            //else { return null; }
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBySql"></param>
        /// <returns></returns>
        public static async Task<Tuple<int, IEnumerable<T>>> PageLoadAsync<T>(MySqlConnection connection, string sql, object param = null, int pageIndex = 1, int pageSize = 20, string sortBySql = "")
        {
            int total = await ExecuteScalarAsync<int>(connection, $"select count(*) {sql.Substring(sql.ToLower().IndexOf("from"))}", param);

            if (!string.IsNullOrEmpty(sortBySql))
                sql += $"{sortBySql}";

            sql += $"LIMIT {pageIndex - 1 * pageSize},{pageSize}";
            var query = await QueryAsync<T>(connection, sql, param);
            return Tuple.Create(total, query);
        }
        #endregion
    }
}