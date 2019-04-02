using Microsoft.EntityFrameworkCore;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
   public partial class TriggerDAL : BaseDAL<TriggerModel>, ITriggerDAL
    {
        private DbContext _db;

        public TriggerDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
