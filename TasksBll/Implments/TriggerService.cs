using Microsoft.Extensions.Configuration;
using SchedulerModel.Entity;
using SchedulerQuartz;
using System;
using System.Collections.Generic;
using System.Text;
using TasksBll.Interface;
using TasksDAL.Interface;
using System.Linq;
using TasksEntity.BM_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulerCommon.Library;
using TasksEntity.Model;

namespace TasksBll.Implments
{
    public partial class TriggerService : BaseService<TriggerModel>, ITriggerService
    {
        private ITriggerDAL _triggerDAL;
        private readonly SchedulerServer _schedulerServer;
        private IConfiguration _configuration { get; }
        public TriggerService(ITriggerDAL triggerDAL, SchedulerServer schedulerServer, IConfiguration configuration)
        {
            _triggerDAL = triggerDAL;
            _schedulerServer = schedulerServer;
            _configuration = configuration;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _triggerDAL;
        }
        /// <summary>
        /// 获取所有的触发器
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList(int page, int PageNum)
        {
            var user = Dal.FindList<UserModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).AsNoTracking().ToList();
            var Trigger = Dal.FindList<TriggerModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).AsNoTracking().ToList();
            var query = from triggerModel in Trigger
                        join userModel in user on triggerModel.UserId equals userModel.Id into t
                        from tempModel in t.DefaultIfEmpty()
                        select new
                        {
                            Id = triggerModel.Id,
                            UserName = tempModel.UserName,
                            UserId = triggerModel.UserId,
                            Name = triggerModel.Name,
                            Value = triggerModel.Value,
                            CreatedTime = triggerModel.CreatedTime,
                            UpdatedTime = triggerModel.UpdatedTime
                        };
            if (!Auth.IsAdmin)
            {
                query = query.Where(rs => rs.UserId == Auth.Info.Id);
            }
            var result = query.OrderByDescending(rs => rs.Id).Skip(PageNum * (page - 1)).Take(PageNum).ToList();
            var total = Auth.IsAdmin
                ? Dal.FindList<TriggerModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).Count()
                : Dal.FindList<TriggerModel>(rs => rs.UserId == Auth.Info.Id).Count();
            return Tools.ReJson(new
            {
                list = result,
                total = total,
                pageNum = PageNum,
                currentPage = page
            });

        }

        /// <summary>
        /// 添加触发器
        /// </summary>
        /// <returns></returns>
        public JsonResult AddDo(string name, string value)
        {
            if (SchedulerServer.FireTimeList(value, 1).Count <= 0)
            {
                return Tools.ReJson("触发器规则不正确");
            }
            TriggerModel triggerModel = new TriggerModel()
            {
                Name = name,
                UserId = Auth.Info.Id,
                Value = value,
                CreatedTime = DateTime.Now.ToLocalTime(),
                UpdatedTime = DateTime.Now.ToLocalTime()
            };
            Dal.Add(triggerModel);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("添加触发器失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 获取触发器信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TriggerModel Edit(int id)
        {
            var result = Auth.IsAdmin
                ? Dal.FindList<TriggerModel>(rs => rs.Id == id).FirstOrDefault()
                : Dal.FindList<TriggerModel>(rs => rs.Id == Auth.Info.Id).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// 修改触发器
        /// </summary>
        /// <returns></returns>
        public JsonResult EditDo(int id, string name, string value)
        {
            if (SchedulerServer.FireTimeList(value, 1).Count <= 0)
            {
                return Tools.ReJson("触发器规则不正确");
            }

            var result = Auth.IsAdmin
                ? Dal.FindList<TriggerModel>(rs => rs.Id == id).FirstOrDefault()
                : Dal.FindList<TriggerModel>(rs => rs.Id == id && rs.UserId == Auth.Info.Id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取触发器信息失败");
            }

            result.Name = name;
            result.Value = value;
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("更新触发器信息失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 删除触发器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelDo(int id)
        {
            var result = Auth.IsAdmin
            ? Dal.FindList<TriggerModel>(rs => rs.Id == id).FirstOrDefault()
            : Dal.FindList<TriggerModel>(rs => rs.Id == id && rs.UserId == Auth.Info.Id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取触发器信息失败");
            }
            Dal.Delete(result);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("删除触发器信息失败");
            }
            return Tools.ReJson();
        }
    }

}
