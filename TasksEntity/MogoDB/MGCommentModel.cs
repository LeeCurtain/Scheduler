using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGCommentModel : MGBaseEntity
    {
        public string TargetId { get; set; }
        public int TargetType { get; set; }
        public MGGlobalUserInfo User { get; set; }
        public string Content { get; set; }
        public int ReplyCount { get; set; }
        public List<MGReplyModel> Replys { get; set; }
        public List<MGNotificationUsers> Notifications { get; set; }
        public int Status { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateDate { get; set; }
    }

    public class MGReplyModel : MGBaseEntity
    {
        public string TargetId { get; set; }
        public MGGlobalUserInfo User { get; set; }
        public string Content { get; set; }
        public MGGlobalUserInfo ToUser { get; set; }
        public List<MGNotificationUsers> Notifications { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateDate { get; set; }
    }

    public class MGNotificationUsers
    {
        public string UserId { get; set; }
        public string NickName { get; set; }
    }
}
