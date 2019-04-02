using System;
using System.Collections.Generic;
using System.Text;
using TasksBll.Interface;
using TasksDAL.Interface;
using TasksEntity.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TasksEntity.MogoDB;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TasksBll.Implments
{
    public partial class UserArticleService : BaseService<UserInfo>, IUserArticleService
    {
        private readonly IUserArticleDAL _userArticleDAL;
        private readonly IMongoDBBaseDAL _mongoDBBaseDAL;
        public UserArticleService(IUserArticleDAL userArticleDAL, IMongoDBBaseDAL mongoDBBaseDAL)
        {
            _userArticleDAL = userArticleDAL;
            _mongoDBBaseDAL = mongoDBBaseDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _userArticleDAL;
        }
        /// <summary>
        /// 图文
        /// </summary>
        // TextPhoto = 1,
        /// <summary>
        /// 视频
        /// </summary>
        // Video = 2,
        /// <summary>
        /// 供需
        /// </summary>
        //SupplyDemand = 3,
        /// <summary>
        /// 想法
        /// </summary>
        //Idea = 4
        /// <summary>
        /// 推荐热门文章
        /// </summary>
        public async Task HostAricleByDayAsync()
        {
            var query = Dal.FindList<RecommendItem>(a => a.Type == 1 && a.Vip >= 1).OrderByDescending(a => a.Grade).ThenByDescending(r => Guid.NewGuid()).Take(10).ToList();
            if (query != null)
            {
                foreach (RecommendItem userArticle in query)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        //更新mogo  
                        FilterDefinition<MGMomentModel> filter = Builders<MGMomentModel>.Filter.Eq(a => a.User.UserId, userArticle.ItemId.ToString().ToLower());
                        if (i != 4)
                        {
                            filter = filter & Builders<MGMomentModel>.Filter.Gt("LikeCount", 1);
                            filter = filter & Builders<MGMomentModel>.Filter.Gt("CommentCount", 1);
                        }
                        filter = filter & Builders<MGMomentModel>.Filter.Eq(x => x.Type, i);
                        //排序
                        SortDefinition<MGMomentModel> sort = null;
                        sort = Builders<MGMomentModel>.Sort.Descending(a => a.CreateDate).Descending(a => a.LikeCount).Descending(a => a.CommentCount);
                        string[] fileds = null;
                        //sort=sort & sort.Descending(a => a.LikeCount)
                        var momentList = await _mongoDBBaseDAL.FindListByPageAsync<MGMomentModel>(MGTable.Moments, filter, 1, 5, fileds, sort);
                        if (momentList.Count > 0)
                        {
                            if (i != 4)
                            {
                                var r = momentList.Where(a => a.LikeCount > 0 && a.CommentCount > 0).OrderByDescending(a => a.CommentCount).ThenByDescending(a => a.LikeCount).FirstOrDefault();
                                if (r != null)
                                {
                                    r.IsHot = true;
                                    //更新文档
                                    _mongoDBBaseDAL.Update<MGMomentModel>(MGTable.Moments, r, r.Id.ToString());
                                }
                            }
                            else
                            {
                                var moment = momentList.Where(a => a.User.Vip > 0).OrderByDescending(a => a.CreateDate).FirstOrDefault();
                                if (moment != null)
                                {
                                    moment.IsHot = true;
                                    //更新文档
                                    _mongoDBBaseDAL.Update<MGMomentModel>(MGTable.Moments, moment, moment.Id.ToString());
                                }
                            }

                        }
                    }

                }
            }
        }
        /// <summary>
        /// 热门推送
        /// </summary>
        /// <returns></returns>
        public async Task HostAricleByDay()
        {
            for (int i = 1; i <= 4; i++)
            {
                //更新mogo  
                FilterDefinition<MGMomentModel> filter = Builders<MGMomentModel>.Filter.Eq(a => a.IsHot, false);
                if (i != 4)
                {
                    filter = filter & Builders<MGMomentModel>.Filter.Gt("LikeCount", 1);
                    filter = filter & Builders<MGMomentModel>.Filter.Gt("CommentCount", 1);
                }
                filter = filter & Builders<MGMomentModel>.Filter.Eq(x => x.Type, i);
                //排序
                SortDefinition<MGMomentModel> sort = null;
                sort = Builders<MGMomentModel>.Sort.Descending(a => a.CreateDate).Descending(a => a.LikeCount).Descending(a => a.CommentCount);
                string[] fileds = null;
                //sort=sort & sort.Descending(a => a.LikeCount)
                var momentList = await _mongoDBBaseDAL.FindListByPageAsync<MGMomentModel>(MGTable.Moments, filter, 1, 5, fileds, sort);
                if (momentList.Count > 0)
                {
                    if (i != 4)
                    {
                        var r = momentList.Where(a => a.LikeCount > 0 && a.CommentCount > 0).OrderByDescending(a => a.CommentCount).ThenByDescending(a => a.LikeCount);
                        if (r.Count()> 0)
                        {
                            foreach (MGMomentModel mG in r)
                            {
                                mG.IsHot = true;
                                //更新文档
                                _mongoDBBaseDAL.Update<MGMomentModel>(MGTable.Moments, mG, mG.Id.ToString());
                            }
                        }
                    }
                    else
                    {
                        var moment = momentList.Where(a => a.User.Vip > 0).OrderByDescending(a => a.CreateDate);
                        if (moment.Count() >0)
                        {
                            foreach (MGMomentModel mG in moment)
                            {
                                mG.IsHot = true;
                                //更新文档
                                _mongoDBBaseDAL.Update<MGMomentModel>(MGTable.Moments, mG, mG.Id.ToString());
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 更新热门
        /// </summary>
        /// <returns></returns>
        public async Task HostAricleByDayUpdate()
        {
            for (int i = 1; i <= 5; i++)
            {
                FilterDefinition<MGMomentModel> filter = Builders<MGMomentModel>.Filter.Eq(a => a.IsHot, true);
                filter = filter & Builders<MGMomentModel>.Filter.Eq(x => x.Type, i);
                //排序
                SortDefinition<MGMomentModel> sort = null;
                sort = Builders<MGMomentModel>.Sort.Descending(a => a.CreateDate);
                string[] fileds = null;
                //sort=sort & sort.Descending(a => a.LikeCount)
                var momentList = await _mongoDBBaseDAL.FindListByPageAsync<MGMomentModel>(MGTable.Moments, filter, 1, 100, fileds, sort);
                if (momentList != null)
                {
                    foreach (MGMomentModel moment in momentList)
                    {
                        moment.IsHot = false;
                        //更新文档
                        _mongoDBBaseDAL.Update<MGMomentModel>(MGTable.Moments, moment, moment.Id.ToString());
                    }
                }
            }
        }

    }
}
