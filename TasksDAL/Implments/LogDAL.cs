using System;
using System.Collections.Generic;
using System.Text;
using SchedulerModel.Entity;
using TasksDAL.Interface;
using Microsoft.EntityFrameworkCore;
using TasksEntity.Model;
namespace TasksDAL.Implments
{
   public partial class LogDAL:BaseDAL<LoggerModel>,ILogDAL
    {
       private DbContext _db;
        public LogDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
