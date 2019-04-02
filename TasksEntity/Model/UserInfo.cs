using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Model
{
    public class UserInfo
    {
       
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Telphone { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public int Vip { get; set; }
        public int Gold { get; set; }
        public decimal Balance { get; set; }
        public int Coupons { get; set; }
        public int Friends { get; set; }
        public int Follows { get; set; }
        public int Followers { get; set; }
        public int Collects { get; set; }
        public int Likes { get; set; }
        public int Approvals { get; set; }
        public int Visitors { get; set; }
        public int Score { get; set; }
        public int ScoreRank { get; set; }
        public int Moments { get; set; }
        public int Articles { get; set; }
        public int Products { get; set; }
        public int Photos { get; set; }
        public int Comments { get; set; }
        public string Industry { get; set; }
        public string FieldTags { get; set; }
        public string HopeTags { get; set; }
        public string BusinessInfo { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public int ZoneStatus { get; set; }
        public int Bnstatus { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public DateTime RegisterAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string InviteUrl { get; set; }
        public int IdentityAuth { get; set; }
        public int CareerAuth { get; set; }
        public int CareerType { get; set; }
    }
}
