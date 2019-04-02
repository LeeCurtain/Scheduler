using System;
using System.Collections.Generic;
using System.Text;
using TasksBll.Interface;
using SchedulerModel.Entity;
using TasksDAL.Interface;
using System.Threading.Tasks;

namespace TasksBll.Implments
{
    public class LogService : BaseService<LoggerModel>, ILogService
    {
        private ILogDAL _logDAL;
        public LogService(ILogDAL logDAL)
        {
            _logDAL = logDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _logDAL;
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logger"></param>
        public async Task AddLog(LoggerModel logger)
        {
           await Dal.AddAsync(logger);
            Dal.SaveChanges();
        }
    }
}
