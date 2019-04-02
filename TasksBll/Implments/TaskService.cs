using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulerCommon.Entity;
using SchedulerCommon.Library;
using SchedulerModel.Entity;
using SchedulerQuartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TasksBll.Interface;
using TasksDAL.Implments;
using TasksDAL.Interface;
using TasksEntity.BM_Models;

namespace TasksBll.Implments
{
    public partial class TaskService : BaseService<TaskModel>, ITaskService
    {
        private ITaskDAL _taskDAL;
        private readonly SchedulerServer _schedulerServer;
        private IConfiguration _configuration { get; }
        public TaskService(ITaskDAL taskDAL, SchedulerServer schedulerServer, IConfiguration configuration)
        {
            _taskDAL = taskDAL;
            _schedulerServer = schedulerServer;
            _configuration = configuration;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _taskDAL;
        }
        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNum"></param>
        public TaskList TaskListFunc(int page, int PageNum)
        {
            var user = Dal.FindList<UserModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).AsNoTracking().ToList();
            var task = Dal.FindList<TaskModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).AsNoTracking().ToList();
            var groupw = Dal.FindList<GroupModel>(a => a.Name != "").AsNoTracking().ToList();
            var query = from taskModel in task
                        join userModel in user on taskModel.UserId equals userModel.Id into t1
                        from t1Model in t1.DefaultIfEmpty()
                        join groupModel in groupw on taskModel.GroupId equals groupModel.Id into t2
                        from t2Model in t2.DefaultIfEmpty()
                        select new TaskInfoEntity
                        {
                            Id = taskModel.Id,
                            Name = taskModel.Name,
                            Description = taskModel.Description,
                            UserId = taskModel.UserId,
                            UserName = t1Model.UserName,
                            TriggerId = taskModel.TriggerId,
                            TriggerValue = taskModel.TriggerValue,
                            TriggerDesc = taskModel.TriggerDesc,
                            Type = taskModel.Type,
                            Value = taskModel.Value,
                            Status = taskModel.Status,
                            CreatedTime = taskModel.CreatedTime,
                            GroupName = t2Model.Name
                        };
            if (!Auth.IsAdmin)
            {
                query = query.Where(rs => rs.UserId == Auth.Info.Id);
            }

            var result = query.OrderByDescending(rs => rs.Id).Skip(PageNum * (page - 1)).Take(PageNum).ToList();
            var total = Auth.IsAdmin
                    ? task.Count()
                    : task.Count(rs => rs.UserId == Auth.Info.Id);
            TaskList list = new TaskList()
            {
                taskInfoEntity = result,
                TaskTotal = total,
                PageNum = PageNum,
                CurrentPage = page,
            };
            return list;
        }
        /// <summary>
        /// 任务的下拉列表
        /// </summary>
        /// <returns></returns>
        public TaskTG TaskAddTG()
        {
            return new TaskTG()
            {
                triggers = Dal.FindList<TriggerModel>(rs => rs.UserId == Auth.Info.Id).OrderByDescending(rs => rs.Id).AsNoTracking().ToList(),
                groups = Dal.FindList<GroupModel>(a => a.Name != "").OrderByDescending(rs => rs.Id).AsNoTracking().ToList(),
            };
        }
        /// <summary>
        /// 编辑任务获取信息
        /// </summary>
        /// <returns></returns>
        public TaskTTG TaskeditTTG(int id)
        {
            var result = Auth.IsAdmin
                  ? Dal.FindList<TaskModel>(rs => rs.Id == id).FirstOrDefault()
                  : Dal.FindList<TaskModel>(rs => rs.Id == id && rs.UserId == Auth.Info.Id).FirstOrDefault();
            return new TaskTTG()
            {
                Task = result,
                triggers = Dal.FindList<TriggerModel>(rs => rs.UserId == Auth.Info.Id).OrderByDescending(rs => rs.Id).AsNoTracking().ToList(),
                groups = Dal.FindList<GroupModel>(a => a.Name != "").OrderByDescending(rs => rs.Id).AsNoTracking().ToList(),
            };
        }
        /// <summary>
        /// 新增加任务
        /// </summary>
        /// <returns></returns>
        public JsonResult AddDo(string name, string desc, string type, string value, string triggerId, string groupId)
        {

            var triggerInfo = Dal.FindList<TriggerModel>(rs => rs.Id == int.Parse(triggerId) && rs.UserId == Auth.Info.Id).FirstOrDefault();
            if (triggerInfo == null)
            {
                return Tools.ReJson("获取触发器信息失败");
            }

            var groupInfo = Dal.FindList<GroupModel>(rs => rs.Id == int.Parse(groupId)).FirstOrDefault();
            if (groupInfo == null)
            {
                return Tools.ReJson("获取任务组信息失败");
            }

            TaskModel task = new TaskModel()
            {
                Name = name,
                GroupId = int.Parse(groupId),
                Description = desc,
                TriggerId = int.Parse(triggerId),
                TriggerValue = triggerInfo.Value,
                TriggerDesc = triggerInfo.Name,
                UserId = Auth.Info.Id,
                Type = int.Parse(type),
                Value = value,
                Status = 0,
                CreatedTime = DateTime.Now.ToLocalTime(),
                UpdatedTime = DateTime.Now.ToLocalTime(),
            };
            Dal.Add(task);
            if (Dal.SaveChanges())
            {
                return Tools.ReJson();
            }
            else
            {
                return Tools.ReJson("添加任务失败");
            }
        }
        /// <summary>
        /// 修改任务
        /// </summary>
        /// <returns></returns>
        public JsonResult EditDo(int id, string name, string desc, int type, string value, int triggerId, int groupId)
        {
            var result = Auth.IsAdmin
                            ? Dal.FindList<TaskModel>(rs => rs.Id == id).FirstOrDefault()
                            : Dal.FindList<TaskModel>(rs => rs.Id == id && rs.UserId == Auth.Info.Id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            if (result.Status == 1)
            {
                return Tools.ReJson("任务正在运行中，请先停用");
            }

            var resetJob = false;
            //如果选择了触发器
            if (triggerId > 0)
            {
                var triggerInfo = Dal.FindList<TriggerModel>(rs => rs.Id == triggerId && rs.UserId == Auth.Info.Id).FirstOrDefault();
                if (triggerInfo == null)
                {
                    return Tools.ReJson("获取触发器信息失败");
                }

                if (Tools.Md5(triggerInfo.Value) != Tools.Msg(result.TriggerValue))
                {
                    result.TriggerValue = triggerInfo.Value;
                    result.Description = triggerInfo.Name;
                    result.TriggerId = triggerId;
                    result.Status = 0;
                    resetJob = true;
                }
            }

            if (type != result.Type)
            {
                result.Type = type;
                resetJob = true;
            }

            if (value != result.Value)
            {
                result.Value = value;
                resetJob = true;
            }

            result.Name = name;
            result.GroupId = groupId;
            result.Description = desc;
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("修改任务失败");
            }
            //如果更改了触发器|任务类型|任务值
            if (resetJob)
            {

            }
            return Tools.ReJson();
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        public JsonResult DelDo(int id)
        {
            var result = Dal.FindList<TaskModel>(rs => rs.Id == id).AsNoTracking().FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }
            Dal.Delete(result);
            if (Dal.SaveChanges())
            {
                return Tools.ReJson(_schedulerServer.RemoveJob(id.ToString()));
            }
            else
            {
                return Tools.ReJson("删除任务信息失败");
            }
        }
        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult StatusDo(int id, int status)
        {
            var result = Dal.FindList<TaskModel>(rs => rs.Id == id).AsNoTracking().FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            result.Status = status;
            Dal.Update(result);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("更改任务信息失败");
            }

            var userInfo = Dal.FindList<UserModel>(rs => rs.Id == result.UserId).AsNoTracking().FirstOrDefault();
            if (userInfo == null)
            {
                return Tools.ReJson("获取用户信息失败");
            }

            var groupInfo = Dal.FindList<GroupModel>(rs => rs.Id == result.GroupId).AsNoTracking().FirstOrDefault();
            if (groupInfo == null)
            {
                return Tools.ReJson("获取任务组信息失败");
            }

            var emailEntity = new EmailEntity();
            _configuration.GetSection("email").Bind(emailEntity);

            if (status == 1)
            {
                _schedulerServer.AddJob(new SchedulerJobEntity
                {
                    Key = result.Id.ToString(),
                    TaskInfo = new TaskInfoEntity
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Description = result.Description,
                        UserId = result.UserId,
                        UserName = userInfo.UserName,
                        UserEmail = userInfo.Email,
                        TriggerId = result.TriggerId,
                        TriggerValue = result.TriggerValue,
                        TriggerDesc = result.TriggerDesc,
                        Type = result.Type,
                        Value = result.Value,
                        Status = result.Status,
                        CreatedTime = result.CreatedTime,
                        GroupName = groupInfo.Name
                    },
                    EmailInfo = emailEntity
                },
                    new SchedulerTriggerEntity
                    {
                        Key = result.Id + "-" + result.TriggerId,
                        Desc = result.TriggerDesc,
                        Rule = result.TriggerValue
                    }).GetAwaiter().GetResult();
            }
            else
            {
                _schedulerServer.RemoveJob(id.ToString()).GetAwaiter().GetResult();
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <returns></returns>
        public JsonResult RunDo(int id)
        {
            var result = Dal.FindList<TaskModel>(rs => rs.Id == id).AsNoTracking().FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            if (result.Status == 0)
            {
                return Tools.ReJson("此任务未启用");
            }
            return Tools.ReJson(_schedulerServer.RunJob(id.ToString()));
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        public JsonResult PauseDo(string id, int taskId)
        {
            var result = Dal.FindList<TaskModel>(rs => rs.Id == taskId).AsNoTracking().FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            if (result.Status == 0)
            {
                return Tools.ReJson("此任务未启用");
            }

            return Tools.ReJson(_schedulerServer.PauseTrigger(id));
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <returns></returns>
        public JsonResult ResumeDo(string id, int taskId)
        {
            var result = Dal.FindList<TaskModel>(rs => rs.Id == taskId).AsNoTracking().FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            if (result.Status == 0)
            {
                return Tools.ReJson("此任务未启用");
            }

            return Tools.ReJson(_schedulerServer.ResumeTrigger(id));
        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLogger(int id, int page, int PageNum)
        {
            var result = Dal.FindList<LoggerModel>(rs => rs.TaskId == id).OrderByDescending(rs => rs.Id)
                .Skip(PageNum * (page - 1)).Take(PageNum).ToList();
            var total = Dal.FindList<LoggerModel>(rs => rs.TaskId == id).Count();
            return Tools.ReJson(new
            {
                list = result,
                total = total,
                pageNum = PageNum,
                currentPage = page
            });
        }
    }
}
