using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGLikeModel : MGBaseEntity
    {
        public string TargetId { get; set; }
        public int TargetType { get; set; }
        public MGGlobalUserInfo User { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
