using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TasksDAL.Interface
{
   public interface IMongoDBBaseDAL
    {
        MongoDB.Driver.IMongoDatabase GetDatabase();
        IMongoCollection<T> GetClient<T>(string collName) where T : class, new();
        int Add<T>(string collName, T t) where T : class, new();
        Task<int> AddAsync<T>(string collName, T t) where T : class, new();
        long Count<T>(string collName, FilterDefinition<T> filter) where T : class, new();
        Task<long> CountAsync<T>(string collName, FilterDefinition<T> filter) where T : class, new();
        DeleteResult Delete<T>(string collName, string id, bool isObjectId = true) where T : class, new();
        Task<DeleteResult> DeleteAsync<T>(string collName, string id, bool isObjectId = true) where T : class, new();
        DeleteResult DeleteMany<T>(string collName, FilterDefinition<T> filter) where T : class, new();
        Task<DeleteResult> DeleteManyAsync<T>(string collName, FilterDefinition<T> filter) where T : class, new();
        List<T> FindList<T>(string collName, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null) where T : class, new();
        Task<List<T>> FindListAsync<T>(string collName, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null) where T : class, new();
        List<T> FindListByPage<T>(string collName, FilterDefinition<T> filter, int pageIndex, int pageSize, out long count, string[] field = null, SortDefinition<T> sort = null) where T : class, new();
        Task<List<T>> FindListByPageAsync<T>(string collName, FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null) where T : class, new();
        T FindOne<T>(string collName, string id, bool isObjectId = true, string[] field = null) where T : class, new();
        Task<T> FindOneAsync<T>(string collName, string id, bool isObjectId = true, string[] field = null) where T : class, new();
        int InsertMany<T>(string collName, List<T> t) where T : class, new();
        Task<int> InsertManyAsync<T>(string collName, List<T> t) where T : class, new();
        UpdateResult Update<T>(string collName, T t, string id, bool isObjectId = true) where T : class, new();
        Task<UpdateResult> UpdateAsync<T>(string collName, T t, string id, bool isObjectId = true) where T : class, new();
        UpdateResult UpdateManay<T>(string collName, Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new();
        Task<UpdateResult> UpdateManayAsync<T>(string collName, Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new();

        Task<bool> AnyAsync<T>(string collName, FilterDefinition<T> filter) where T : class, new();

        bool Any<T>(string collName, FilterDefinition<T> filter) where T : class, new();
    }
}
