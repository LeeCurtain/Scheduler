using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Model
{
    public partial class BnCoverageRate
    {
        public Guid UserId { get; set; }
        public decimal Crate { get; set; }
        public int FriendsCount { get; set; }
        public int NetUserCount { get; set; }
        public int BuserCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
