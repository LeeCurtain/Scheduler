using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    /// <summary>
    /// MongoDB实体类的基类
    /// </summary>
    public class MGBaseEntity
    {
        /// <summary>
        /// 基类对象的ID，MongoDB要求每个实体类必须有的主键
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId), BsonId]
        public ObjectId Id { get; set; }
    }
}
