using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TasksDAL.Interface;
using TasksEntity.Model;
namespace TasksDAL.Implments
{
    public delegate object EFCallBackHandler(DbContext em);
    public delegate TResult EFSelectCallBackHandler<IQueryable, TResult>(IQueryable query);
    public partial class BaseDAL<T> : IBaseDAL<T> where T : class, new()
    {
        private DbContext dbContext;

        public BaseDAL(SqlContext zDDBDEVContext)
        {
            dbContext = zDDBDEVContext;
        }

        public void Add(T t)
        {
            dbContext.Set<T>().Add(t);
        }

        public async Task AddAsync(T t)
        {
            await dbContext.Set<T>().AddAsync(t);
        }
        public void Delete(T t)
        {
            dbContext.Set<T>().Remove(t);
        }


        public void Update(T t)
        {
            dbContext.Set<T>().Update(t);
        }

        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="obj">待添加的对象</param>
        public void AddObject<T>(T obj) where T : class, new()
        {
            dbContext.Set<T>().Add(obj);
        }

        public async Task AddObjectAsync<T>(T obj) where T : class, new()
        {
            await dbContext.Set<T>().AddAsync(obj);
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="obj">待删除的对象</param>
        public void DeleteObject<T>(T obj) where T : class, new()
        {
            dbContext.Set<T>().Remove(obj);
        }

        /// <summary>
        /// 根据实体的实体键修改实体
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateObject<T>(T obj) where T : class, new()
        {
            dbContext.Set<T>().Update(obj);
        }

        /// <summary>
        /// 获得一个实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="keyValue">主键值</param>
        /// <returns>查询到的对象</returns>
        public virtual T GetEntityByKey<T>(params object[] keyValue) where T : class
        {
            return dbContext.Set<T>().Find(keyValue);
        }

        public virtual async Task<T> GetEntityByKeyAsync<T>(params object[] keyValue) where T : class
        {
            return await dbContext.Set<T>().FindAsync(keyValue);
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>符号条件的实体的集合</returns>
        public IQueryable<T> FindList<T>(Expression<Func<T, bool>> where) where T : class
        {

            return dbContext.Set<T>().Where<T>(where).AsQueryable();

        }

        /// <summary>
        /// 根据多个条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>符合条件的实体的集合</returns>
        public IQueryable<T> FindList<T>(params Expression<Func<T, bool>>[] where) where T : class
        {
            if (where != null && where.Length > 0)
            {
                IQueryable<T> query = dbContext.Set<T>().Where(where[0]);
                for (int i = 1; i < where.Length; i++)
                {
                    query = query.Where(where[i]);
                }
                return query.AsQueryable<T>();
            }
            return null;
        }

        /// <summary>
        /// 根据条件、排序、投影进行查找
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">排序属性类型</typeparam>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="where">查询条件树</param>
        /// <param name="orderSelector">排序</param>
        /// <param name="selector">投影</param>
        /// <returns>符合条件的对象的集合</returns>
        public IQueryable<TResult> FindList<T, TKey, TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderSelector, Expression<Func<T, TResult>> selector) where T : class
        {
            return dbContext.Set<T>().Where<T>(where).OrderBy(orderSelector).Select(selector).AsQueryable();

        }

        /// <summary>
        /// 根据条件和排序进行查找,带抓取功能
        /// </summary>
        /// <typeparam name="T">被查询的实体类型</typeparam>
        /// <typeparam name="TKey">排序属性的类型</typeparam>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="include">抓取属性</param>
        /// <param name="orderSelector">排序选择器</param>
        /// <param name="selector">投影选择器</param>
        /// <returns></returns>
        public IQueryable<TResult> FindList<T, TKey, TResult>(Expression<Func<T, bool>> where, string include, Expression<Func<T, TKey>> orderSelector, Expression<Func<T, TResult>> selector) where T : class
        {
            return dbContext.Set<T>().Include(include).Where<T>(where).OrderBy(orderSelector).Select(selector).AsQueryable();

        }

        /// <summary>
        /// 执行原始SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlNonQuery(string commandText, params DbParameter[] parameters)
        {

            return dbContext.Database.ExecuteSqlCommand(commandText, parameters);
        }

        /// <summary>
        /// 执行任何操作
        /// </summary>
        /// <param name="callBackHandler"></param>
        /// <returns></returns>
        protected object Execute(EFCallBackHandler callBackHandler)
        {
            return callBackHandler(dbContext);
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="conn">连接对象</param>
        /// <param name="tableName">将泛型集合插入到本地数据库表的表名</param>
        /// <param name="list">要插入大泛型集合</param>
        public  void BulkInsert<T>(SqlConnection conn, string tableName, IList<T> list) where T : class
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))

                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }
    }
}

