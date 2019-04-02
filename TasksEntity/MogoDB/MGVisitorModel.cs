using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGVisitorModel : MGBaseEntity
    {
        public MGGlobalUserInfo Visitor { get; set; }

        public string UserId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime VisitDate { get; set; }
    }
}
