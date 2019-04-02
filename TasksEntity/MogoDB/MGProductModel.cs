using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGProductModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal UnitPrice { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Cover { get; set; }
        public int IsHot { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CollectCount { get; set; }
        public int CommentCount { get; set; }
        public string Tags { get; set; }
        public int Status { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateDate { get; set; }
    }
}
