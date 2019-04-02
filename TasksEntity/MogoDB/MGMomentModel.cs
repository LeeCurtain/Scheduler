using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MGMomentModel : MGBaseEntity
    {

        /// <summary>
        /// 发布者信息
        /// </summary>
        public MGGlobalUserInfo User { get; set; }
        /// <summary>
        /// 文章类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 媒体地址
        /// </summary>
        public string Medias { get; set; }
        /// <summary>
        /// 热门
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public System.DateTime UpdateDate { get; set; }

        /// <summary>
        /// 说明：例如 五人已加入
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 可见类型0:开放，1:仅好友可见
        /// </summary>
        public int Visible { get; set; }

        /// <summary>
        /// 评论内容实体
        /// </summary>
        public List<MGCommentModel> Comments { get; set; }
        /// <summary>
        /// 点赞列表
        /// </summary>
        public List<MGLikeModel> Likes { get; set; }
    }
}
