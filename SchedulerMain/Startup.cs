using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver.V1;
using SchedulerCommon.Entity;
using SchedulerCommon.Filter;
using SchedulerModel.Entity;
using SchedulerQuartz;
using SchedulerSignalR;
using TasksBll;
using TasksBll.Interface;
using TasksDAL;
using TasksEntity.Model;
using TasksEntity.MogoDB;

namespace SchedulerMain
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public  IUserLoginService _userLoginService;
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SqlServerConn sqlServerConn = new SqlServerConn();
            MongoDBConn mongoDBConn = new MongoDBConn();
            Neo4jConn neo4JConn = new Neo4jConn();
            Configuration.GetSection("SqlServer").Bind(sqlServerConn);
            Configuration.GetSection("MongoDB").Bind(mongoDBConn);
            Configuration.GetSection("Neo4j").Bind(neo4JConn);
            services.AddMvc(options => { options.Filters.Add<ExceptionFilter>(); })
                .AddJsonOptions(options => { options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSession(session => { session.IdleTimeout = TimeSpan.FromMinutes(30); });
            services.AddSignalR();
            services.AddSingleton<SchedulerExtHub<SchedulerHub>>();
            services.AddSingleton<SchedulerQuartz.SchedulerServer>();

            DIBLLRegister bLLDIRegister = new DIBLLRegister();
            bLLDIRegister.DIRegister(services);

            DALDIRegister dALDIRegister = new DALDIRegister();
            dALDIRegister.DIRegister(services);
            services.AddDbContext<SqlContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer("Server=" + sqlServerConn.Host + "," + sqlServerConn.Port + "; Database=" + sqlServerConn.DataBase + ";Persist Security Info=True;User ID=" + sqlServerConn.UserName + ";password=" + sqlServerConn.Password + ";", a => a.UseRowNumberForPaging());
            }, ServiceLifetime.Scoped);
            //services.AddDbContext<BaseModel>(options =>
            //{
            //    options.UseLazyLoadingProxies().UseSqlServer("Server=" + sqlServerConn.Host + "," + sqlServerConn.Port + "; Database=" + sqlServerConn.DataBase + ";Persist Security Info=True;User ID=" + sqlServerConn.UserName + ";password=" + sqlServerConn.Password + ";", a => a.UseRowNumberForPaging());
            //}, ServiceLifetime.Scoped);
            services.AddTransient<MongoDataBaseContext>(option =>
            {
                return new MongoDataBaseContext(new MongodbHost() { Host = mongoDBConn.Host, Port = mongoDBConn.Port, UserName = mongoDBConn.UserName, PassWord = mongoDBConn.Password, DataBase = mongoDBConn.DataBase });
            });
            services.AddTransient(option =>
            {
                return GraphDatabase.Driver("bolt://" + neo4JConn.Host + ":" + neo4JConn.Port, AuthTokens.Basic(neo4JConn.UserName, neo4JConn.Password));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IUserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
            SiteConfig.SetAppSetting(Configuration.GetSection("SqlServer"));
            //记录真实ip
            app.UseMiddleware<RealIpMiddleware>();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{controller=Index}/{action=Index}"); });
            app.UseSignalR(routes => { routes.MapHub<SchedulerHub>("/scheduler"); });
            var extHub = app.ApplicationServices.GetService<SchedulerExtHub<SchedulerHub>>();
            app.ApplicationServices.GetService<SchedulerQuartz.SchedulerServer>().AddJobs(GetTaskList(), extHub).GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// 初次运行时读取所有可运行任务列表载入任务调度器
        /// </summary>
        /// <returns></returns>
        private Dictionary<SchedulerJobEntity, SchedulerTriggerEntity> GetTaskList()
        {
            var emailEntity = new EmailEntity();
            Configuration.GetSection("email").Bind(emailEntity);
            var dic = new Dictionary<SchedulerJobEntity, SchedulerTriggerEntity>();
            var query = _userLoginService.GetTaskList();
                foreach (var infoEntity in query.Where(rs => rs.Status == 1))
                {
                    dic.Add(
                        new SchedulerJobEntity
                        {
                            Key = infoEntity.Id.ToString(),
                            TaskInfo = infoEntity,
                            EmailInfo = emailEntity
                        },
                        new SchedulerTriggerEntity
                        {
                            Key = infoEntity.Id + "-" + infoEntity.TriggerId,
                            Desc = infoEntity.TriggerDesc,
                            Rule = infoEntity.TriggerValue
                        }
                    );
                }
            return dic;
        }
    }
}
