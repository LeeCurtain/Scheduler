using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TasksEntity.Model;

namespace TasksEntity.MogoDB
{
    public class MGZoneModel
    {
        [BsonId]
        public string _id { get; set; }

        public List<MGVisitorModel> Visitors { get; set; }

        public List<UserCareer> Career { get; set; }

        public List<MGAlbumModel> Albums { get; set; }

        public List<MGProductModel> Products { get; set; }

        public List<MGArticleModel> Articles { get; set; }

        public List<MGApprovalModel> Approvals { get; set; }
    }
}
