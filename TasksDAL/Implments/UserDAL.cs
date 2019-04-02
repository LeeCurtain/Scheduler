using Microsoft.EntityFrameworkCore;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
   public partial class UserDAL : BaseDAL<UserModel>, IUserDAL
    {
        private DbContext _db;

        public UserDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
