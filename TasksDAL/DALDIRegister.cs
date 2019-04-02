using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Implments;
using TasksDAL.Interface;
using Microsoft.Extensions.DependencyInjection;
namespace TasksDAL
{
   public class DALDIRegister
    {
        public void DIRegister(IServiceCollection services)
        {
            services.AddTransient(typeof(IMongoDBBaseDAL), typeof(MongoDBBaseDAL));
            services.AddTransient(typeof(INeo4jBaseDAL), typeof(Neo4jBaseDAL));
            services.AddTransient(typeof(IBaseDAL<>), typeof(BaseDAL<>));
            services.AddTransient(typeof(IBusinessNetDAL),typeof(BusinessNetDAL));
            services.AddTransient(typeof(IRecommendDAL), typeof(RecommendDAL));
            services.AddTransient(typeof(IUserArticleDAL), typeof(UserArticleDAL));
            services.AddTransient(typeof(IUserLoginDAL), typeof(UserLoginDAL));
            services.AddTransient(typeof(ITaskDAL), typeof(TaskDAL));
            services.AddTransient(typeof(ITriggerDAL), typeof(TriggerDAL));
            services.AddTransient(typeof(IGroupDAL), typeof(GroupDAL));
            services.AddTransient(typeof(IUserDAL), typeof(UserDAL));
            services.AddTransient(typeof(ILogDAL), typeof(LogDAL));
        }
    }
}
