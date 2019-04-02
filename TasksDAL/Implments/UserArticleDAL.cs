using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
    public partial class UserArticleDAL : BaseDAL<UserInfo>, IUserArticleDAL
    {
        private DbContext _db;
        public UserArticleDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
