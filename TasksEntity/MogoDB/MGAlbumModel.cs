using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGAlbumModel : MGBaseEntity
    {
        public string UserId { get; set; }
        public string Photo { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
        public string Remark { get; set; }
    }
}
