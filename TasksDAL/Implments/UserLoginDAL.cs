using Microsoft.EntityFrameworkCore;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
  public partial  class UserLoginDAL:BaseDAL<UserModel>, IUserLoginDAL
    {
        private DbContext _db;
        public UserLoginDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
