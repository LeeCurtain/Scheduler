using System;
using System.Collections.Generic;
using System.Text;

namespace TasksBll.Interface
{
    public partial interface IRecommendService
    {
        /// <summary>
        /// 今日推荐 推荐类型 1今日 2商友推荐
        /// </summary>
         void TodayRemm();
        /// <summary>
        /// 商友推荐
        /// </summary>
        void RecommFriends();
    }
}
