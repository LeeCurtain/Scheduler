using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.BM_Models
{
   public class RecommendModel
    {
        public string Position { get; set; }
        public string AvatarUrl { get; set; }
        public string FieldTags { get; set; }
        public string NickName { get; set; }
        public string UserID { get; set; }
        public string Vip { get; set; }
        public string Company { get; set; }
        public string BusinessInfo { get; set; }
        public int IdentityAuth { get; set; }
        public int CareerAuth { get; set; }
        public int CareerType { get; set; }

    }
}
