using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TasksBll.Interface;

namespace TasksBll.Implments
{
   public class WriteLog
    {
        private ILogService _logService;
        public WriteLog(ILogService logService)
        {
            _logService = logService;
        }
        public async Task InsertLogger(LoggerModel logger)
        {
           await _logService.AddLog(logger);
        }
    }
}
