using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    /// <summary>
    /// 全局用户信息实体
    /// </summary>
    public class MGGlobalUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 生意信息
        /// </summary>
        public string BusinessInfo { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        public string Industry { get; set; }
        /// <summary>
        /// 专业领域标签
        /// </summary>
        public string FieldTags { get; set; }
        /// <summary>
        /// VIP等级
        /// </summary>
        public int Vip { get; set; }
        /// <summary>
        /// 实名认证状态
        /// </summary>
        public int IdentityAuth { get; set; }
        /// <summary>
        /// 职业身份状态
        /// </summary>
        public int CareerAuth { get; set; }

        public int CareerType { get; set; }
    }
}
