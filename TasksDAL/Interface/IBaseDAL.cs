using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TasksDAL.Interface
{
    public partial interface IBaseDAL<T> where T : class, new()
    {
        void Add(T t);

        Task AddAsync(T t);
        void Delete(T t);
        void Update(T t);
        bool SaveChanges();

        Task<bool> SaveChangesAsync();
        void AddObject<T>(T obj) where T : class, new();

        Task AddObjectAsync<T>(T obj) where T : class, new();
        void DeleteObject<T>(T obj) where T : class, new();
        void UpdateObject<T>(T obj) where T : class, new();
        T GetEntityByKey<T>(params object[] keyValue) where T : class;

        Task<T> GetEntityByKeyAsync<T>(params object[] keyValue) where T : class;

        int ExecuteSqlNonQuery(string commandText, params DbParameter[] parameters);

        IQueryable<TResult> FindList<T, TKey, TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderSelector, Expression<Func<T, TResult>> selector) where T : class;
        IQueryable<TResult> FindList<T, TKey, TResult>(Expression<Func<T, bool>> where, string include, Expression<Func<T, TKey>> orderSelector, Expression<Func<T, TResult>> selector) where T : class;
        IQueryable<T> FindList<T>(Expression<Func<T, bool>> where) where T : class;
        IQueryable<T> FindList<T>(params Expression<Func<T, bool>>[] where) where T : class;
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="conn">连接对象</param>
        /// <param name="tableName">将泛型集合插入到本地数据库表的表名</param>
        /// <param name="list">要插入大泛型集合</param>
        void BulkInsert<T>(SqlConnection conn, string tableName, IList<T> list) where T : class;

    }
}
