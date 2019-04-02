using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TasksBll.Interface;
using TasksDAL.Interface;
using TasksEntity.BM_Models;
using TasksEntity.Model;

namespace TasksBll.Implments
{
    public partial class RecommendService : BaseService<RecommendItem>, IRecommendService
    {
        private IRecommendDAL _recommendDAL;
        public RecommendService(IRecommendDAL recommendDAL)
        {
            _recommendDAL = recommendDAL;
            SetDal();
        }

        public override void SetDal()
        {
            Dal = _recommendDAL;
        }

        /// <summary>
        /// 今日推荐 推荐类型 1今日 2商友推荐
        /// </summary>
        public void TodayRemm()
        {
            Expression<Func<UserInfo, bool>> exuser = x => x.Status == 1;
            var userinfo = Dal.FindList<UserInfo>(a => a.Status == 1);
            //遍历用户计算综合分值
            foreach (UserInfo userInfo in userinfo)
            {
                //计算每个用户的综合分值
                decimal toal = 0;
                //判断用户是否会员
                if (userInfo.Vip != 0)
                {
                    toal = userInfo.Vip * 10;
                }
                else
                {
                    toal = 10;
                }
                //判断是否开通空间
                if (userInfo.ZoneStatus != 0)
                {
                    toal += 10;
                }
                else
                {
                    toal += 5;
                }
                //名片认证
                if (userInfo.CareerAuth == 1)
                {
                    toal += 10;
                }
                //else if (userInfo.CareerAuth == 2)
                //{
                //    toal -= 5;
                //}
                //else
                //{
                //    toal -= 10;
                //}
                //身份认证
                if (userInfo.IdentityAuth == 1)
                {
                    toal += 10;
                }
                //else if (userInfo.IdentityAuth == 2)
                //{
                //    toal -= 5;
                //}
                //else
                //{
                //    toal -= 10;
                //}
                //if (string.IsNullOrEmpty(userInfo.FieldTags))
                //{
                //    toal -= 5;
                //}
                //RecommendModel rec = new RecommendModel();
                //rec.Position = userInfo.Position;
                //rec.AvatarUrl = userInfo.AvatarUrl;
                //rec.FieldTags = userInfo.FieldTags;
                //rec.NickName = userInfo.NickName;
                //rec.UserID = userInfo.UserId.ToString();
                //rec.Vip = userInfo.Vip.ToString();
                //rec.Company = userInfo.Company;
                //rec.BusinessInfo = userInfo.BusinessInfo;
                //rec.CareerAuth = userInfo.CareerAuth;
                //rec.CareerType = userInfo.CareerType;
                //rec.IdentityAuth = userInfo.IdentityAuth;
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
                //判断用户是否已经存在
                if (Dal.FindList<RecommendItem>(a => a.ItemId == userInfo.UserId && a.Type == 1).Any())
                {
                    //跟新
                    var recm = Dal.FindList<RecommendItem>(a => a.ItemId == userInfo.UserId && a.Type == 1).FirstOrDefault();
                    //判断是否已经推过
                    if (recm.IsRecomm == 1)
                    {
                        recm.Vip = userInfo.Vip;
                        recm.Grade = toal;
                        recm.Tags = userInfo.FieldTags;
                    }
                    else
                    {
                        recm.Vip = userInfo.Vip;
                        recm.Grade = toal;
                        recm.UpdateDate = DateTime.Now;
                        recm.Tags = userInfo.FieldTags;
                    }
                    Dal.Update(recm);
                    //recm.Payload = JsonConvert.SerializeObject(rec, Formatting.None, setting);
                    // Dal.SaveChanges();                       
                }
                else
                {


                    //新增
                    RecommendItem recommendItem = new RecommendItem()
                    {
                        ItemId = userInfo.UserId,
                        Type = 1,
                        Vip = userInfo.Vip,
                        Tags = userInfo.FieldTags,
                        //Payload= JsonConvert.SerializeObject(rec, Formatting.None, setting),
                        Grade = toal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsRecomm = 0,
                    };
                    Dal.Add(recommendItem);
                    //Dal.SaveChanges();
                }
            }
            UpdateThree();
            //RecommFriends();
        }

        public void UpdateThree()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            //判断用户是否为会员 推荐不同数据
            Expression<Func<RecommendItem, bool>> expuser = x => x.Type == 1 && x.IsRecomm == 1 && Convert.ToDateTime(x.UpdateDate).ToString("yyyy-MM-dd") == date;

            List<RecommendItem> items = Dal.FindList<RecommendItem>(expuser).OrderByDescending(a => a.Grade).AsNoTracking().Take(3).ToList();
            if (items.Count != 3)
            {
                //随机取三位推荐位
                var data = Dal.FindList<RecommendItem>(a => a.Type == 1 && a.Vip == 1 && a.IsRecomm != 1).OrderByDescending(a => a.Grade).ThenByDescending(r => Guid.NewGuid()).Take(3);
                if (data.Count() == 3)
                {
                    foreach (RecommendItem item in data)
                    {
                        item.IsRecomm = 1;
                        item.UpdateDate = DateTime.Now;
                    }
                }
                else
                {
                    var query = Dal.FindList<RecommendItem>(a => a.IsRecomm == 1).OrderByDescending(a => a.Grade).ToList();
                    foreach (RecommendItem item in query)
                    {
                        item.IsRecomm = 0;
                        item.UpdateDate = DateTime.Now;
                        Dal.Update(item);
                    }
                    //随机取三位推荐位
                    var dataNew = Dal.FindList<RecommendItem>(a => a.Type == 1 && a.Vip == 1 && a.IsRecomm != 1).OrderByDescending(a => a.Grade).ThenByDescending(r => Guid.NewGuid()).Take(3);
                    foreach (RecommendItem item in dataNew)
                    {
                        item.IsRecomm = 1;
                        item.UpdateDate = DateTime.Now;
                    }
                }
            }

            Dal.SaveChanges();
        }
        /// <summary>
        /// 商友推荐
        /// </summary>
        public void RecommFriends()
        {
            var data = Dal.FindList<RecommendItem>(a => a.Type == 1 && a.Grade >= 0 && a.Vip > 0).OrderByDescending(a => a.Grade).ThenByDescending(r => Guid.NewGuid()).Take(200);
            foreach (RecommendItem recommendItem in data)
            {
                //判断用户是否已经存在
                if (Dal.FindList<RecommendItem>(a => a.ItemId == recommendItem.ItemId && a.Type == 2).Any())
                {
                    //跟新
                    var recm = Dal.FindList<RecommendItem>(a => a.ItemId == recommendItem.ItemId && a.Type == 2).FirstOrDefault();
                    recm.Vip = recommendItem.Vip;
                    recm.Grade = recommendItem.Grade;
                    recm.UpdateDate = DateTime.Now;
                    recm.Tags = recommendItem.Tags;
                    recm.IsRecomm = 0;
                }
                else
                {
                    //新增
                    RecommendItem recommendItems = new RecommendItem()
                    {
                        ItemId = recommendItem.ItemId,
                        Type = 2,
                        Vip = recommendItem.Vip,
                        Tags = recommendItem.Tags,
                        Grade = recommendItem.Grade,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsRecomm = 0,
                    };
                    Dal.Add(recommendItems);
                }
            }
            Dal.SaveChanges();
        }
    }
}
