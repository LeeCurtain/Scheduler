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
    public partial class GroupService : BaseService<GroupModel>, IGroupService
    {
        private IGroupDAL _groupDAL;
        public GroupService(IGroupDAL groupDAL)
        {
            _groupDAL = groupDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _groupDAL;
        }
        /// <summary>
        /// 获取所有分组
        /// </summary>
        /// <returns></returns>
        public List<GroupModel> GetList()
        {
            return Dal.FindList<GroupModel>(a => a.Name != "").OrderByDescending(rs => rs.Id).ToList();
        }
        /// <summary>
        /// 新增分组
        /// </summary>
        /// <returns></returns>
        public JsonResult AddDo(string name)
        {
            if (Dal.FindList<GroupModel>(rs => rs.Name == name).AsNoTracking().Any())
            {
                return Tools.ReJson("任务组名称已存在");
            }

            GroupModel groupModel = new GroupModel()
            {
                Name = name
            };
            Dal.Add(groupModel);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("添加任务组失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 修改组名
        /// </summary>
        /// <returns></returns>
        public JsonResult EditDo(int id, string name)
        {
            if (Dal.FindList<GroupModel>(rs => rs.Id != id && rs.Name == name).AsNoTracking().Any())
            {
                return Tools.ReJson("任务组名称已存在");
            }

            var result = Dal.FindList<GroupModel>(rs => rs.Id == id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务组信息失败");
            }

            result.Name = name;
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("更新任务组信息失败");
            }
            return Tools.ReJson();
        }

        /// <summary>
        /// 删除组
        /// </summary>
        /// <returns></returns>
        public JsonResult DelDo(int id)
        {
            if (Dal.FindList<TaskModel>(rs => rs.GroupId == id).AsNoTracking().Any())
            {
                return Tools.ReJson("该任务组存在下属任务不可删除");
            }

            var result = Dal.GetEntityByKey<GroupModel>(id);
            if (result == null)
            {
                return Tools.ReJson("获取任务组信息失败");
            }
            Dal.Delete(result);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("删除任务组信息失败");
            }
            return Tools.ReJson();
        }
    }
}
