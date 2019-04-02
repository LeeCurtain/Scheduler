using Microsoft.EntityFrameworkCore;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
   public partial class TaskDAL : BaseDAL<TaskModel>, ITaskDAL
    {
        private DbContext _db;

        public TaskDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
