using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TasksBll.Interface
{
    public interface ILogService
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logger"></param>
          Task AddLog(LoggerModel logger);
    }
}
