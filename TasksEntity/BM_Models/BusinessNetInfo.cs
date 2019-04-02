using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.BM_Models
{
    public class BusinessNetInfo
    {
        public int UserCount { get; set; }

        public BusinessNetUser BusinessNetUser { get; set; }

        public string Description { get; set; }
    }

    public class BusinessNetUser
    {

        public Guid UserId { get; set; }
        public string Avatar_URL { get; set; }
        public DateTime CreateDate { get; set; }
    }


    /// <summary>
    /// 生意网络覆盖率实体
    /// </summary>
    public class BNCoverageRateModel
    {
        /// <summary>
        /// 生意网络覆盖率
        /// </summary>
        public decimal CoverageRate { get; set; }
        /// <summary>
        /// 覆盖等级
        /// </summary>
        public string CRLevel { get; set; }
        /// <summary>
        /// 全网用户数
        /// </summary>
        public int UserCount { get; set; }
    }
}
