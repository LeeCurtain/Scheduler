using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TasksBll.Interface;
using TasksEntity.Model;
using TasksDAL.Interface;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TasksEntity.BM_Models;
using TasksCommon;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace TasksBll.Implments
{
    public class BusinessNetService : BaseService<UserInfo>, IBusinessNetService
    {
        private IBusinessNetDAL _businessNetDAL;
        private INeo4jBaseDAL _neo4JBaseDAL;
        public BusinessNetService(IBusinessNetDAL businessNetDAL, INeo4jBaseDAL neo4JBaseDAL)
        {
            _businessNetDAL = businessNetDAL;
            _neo4JBaseDAL = neo4JBaseDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _businessNetDAL;
        }
        public string BusinessNetWorkInfo()
        {
            Expression<Func<UserInfo, bool>> exp = x => x.Status == 1;
            var query = Dal.FindList<UserInfo>(exp).OrderByDescending(a => a.RegisterAt);
            int count = query.Count();
            UserInfo userInfo = query.FirstOrDefault();
            BusinessNetInfo businessNetInfo = new BusinessNetInfo()
            {
                UserCount = count,
                BusinessNetUser = new BusinessNetUser() { UserId = userInfo.UserId, Avatar_URL = userInfo.AvatarUrl, CreateDate = userInfo.RegisterAt },
                Description = Functions.GetTimeSpan(userInfo.RegisterAt, DateTime.Now) + "加入"
            };
            return JsonConvert.SerializeObject(businessNetInfo);
        }
        /// <summary>
        /// 生意网络统计
        /// </summary>
        /// <returns></returns>
        public string BusinessNet()
        {
            string msg;
            try
            {
                int start = 2;
                int deep = 4;//查询深度
                int limit = int.MaxValue;
                Expression<Func<UserInfo, bool>> exp = x => x.Status == 1;
                var query = Dal.FindList<UserInfo>(exp).OrderByDescending(a => a.RegisterAt);
                int count = query.Count();
                 
                //获取单个用户的好友数
                foreach (UserInfo userInfo in query)
                {
                    int friends = _neo4JBaseDAL.GetFriendsCount(userInfo.UserId);
                    List<string> list = _neo4JBaseDAL.GetUserBusinessNetworkUsers(userInfo.UserId, start, deep, "", "");                 
                    decimal Crate = (decimal)list.Count / count * 100;
                    //判断用户是否已存在 
                    var bUser = Dal.GetEntityByKey<BnCoverageRate>(userInfo.UserId);
                    if (bUser != null)
                    {

                        bUser.BuserCount = count;
                        bUser.NetUserCount = list.Count;
                        bUser.FriendsCount = friends;
                        bUser.Crate = Crate;
                        bUser.UpdateDate = DateTime.Now;
                        // Dal.SaveChanges();
                    }
                    else
                    {
                        BnCoverageRate bnCoverageRate = new BnCoverageRate()
                        {
                            UserId = userInfo.UserId,
                            Crate = Crate,
                            NetUserCount = list.Count,
                            FriendsCount=friends,
                            BuserCount = count,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                        };
                        Dal.AddObject(bnCoverageRate);
                        // Dal.SaveChanges();
                    }
                }
                Dal.SaveChanges();
               
                return msg = "生意网络统计执行成功";
            }
            catch (Exception e)
            {
                return e.Message;
                throw;
            }

        }
      
    }
}
