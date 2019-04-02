using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TasksBll.Implments;
using TasksBll.Interface;

namespace TasksBll
{
    public class DIBLLRegister
    {
        public void DIRegister(IServiceCollection services)
        {
            services.AddTransient(typeof(IBusinessNetService), typeof(BusinessNetService));
            services.AddTransient(typeof(IRecommendService), typeof(RecommendService));
            services.AddTransient(typeof(IUserArticleService), typeof(UserArticleService));
            services.AddTransient(typeof(IUserLoginService), typeof(UserLoginService));
            services.AddTransient(typeof(ITaskService), typeof(TaskService));
            services.AddTransient(typeof(ITriggerService), typeof(TriggerService));
            services.AddTransient(typeof(IGroupService), typeof(GroupService));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(ILogService), typeof(LogService));
        }
    }
}
