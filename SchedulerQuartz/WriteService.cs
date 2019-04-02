using Microsoft.Extensions.Configuration;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using TasksCommon;

namespace SchedulerQuartz
{
   public class WriteService
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public  async Task InsertLogger(LoggerModel logger)
        {
            SqlConn sqlServerConn = new SqlConn();
            sqlServerConn.Host= SiteConfig.GetSite("Host");
            sqlServerConn.Port = SiteConfig.GetSite("Port");
            sqlServerConn.DataBase = SiteConfig.GetSite("DataBase");
            sqlServerConn.UserName = SiteConfig.GetSite("UserName");
            sqlServerConn.Password = SiteConfig.GetSite("Password");
            using (var db = new BaseModel(sqlServerConn))
            {
                await db.Logger.AddAsync(logger);
                db.SaveChanges();
            }
        }
    }
}
