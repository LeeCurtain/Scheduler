using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TasksDAL.Interface;
using TasksEntity.Model;

namespace TasksDAL.Implments
{
    public partial class RecommendDAL :BaseDAL<RecommendItem>, IRecommendDAL
    {
        private DbContext _db;
        public RecommendDAL(SqlContext zDDBDEVContext) : base(zDDBDEVContext)
        {
            _db = zDDBDEVContext;
        }
    }
}
