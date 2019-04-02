using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;
namespace TasksDAL.Implments
{
   public class BusinessNetDAL:BaseDAL<UserInfo>, IBusinessNetDAL
    {
        private DbContext _db;

        public BusinessNetDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
