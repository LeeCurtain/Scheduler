using System;
using System.Collections.Generic;
using System.Text;

namespace TasksCommon
{
    /// <summary>
    /// 查询排序与分页基类
    /// </summary>
    public class PaginationBase
    {
        private int _pageSize = 10;
        private int MaxPageSize { get; set; } = 100;

        /// <summary>
        /// 当前页，初始页：1
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 页容量 默认10，最大100
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// 排序方式 默认倒序
        /// </summary>
        public string Order { get; set; } = "DESC";

        /// <summary>
        /// 排序字段
        /// </summary>
        public virtual string SortBy { get; set; } = "";//= nameof(IEntity.KeyID);

        /// <summary>
        /// 查询的字段 (逗号分隔)
        /// </summary>
        public string Fields { get; set; }
    }
}
