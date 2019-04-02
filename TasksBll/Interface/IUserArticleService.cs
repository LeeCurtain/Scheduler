using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TasksBll.Interface
{
    public partial interface IUserArticleService
    {
        /// <summary>
        /// 推荐热门文章
        /// </summary>
         Task HostAricleByDayAsync();
        /// <summary>
        /// 更新热门
        /// </summary>
        /// <returns></returns>
        Task HostAricleByDayUpdate();
        /// <summary>
        /// 热门推送
        /// </summary>
        /// <returns></returns>
        Task HostAricleByDay();
    }
}
