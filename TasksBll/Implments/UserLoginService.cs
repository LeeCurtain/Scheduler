using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksBll.Interface;
using TasksDAL.Interface;
using System.Linq;
using TasksEntity.Model;
using SchedulerCommon.Library;
using SchedulerCommon.Entity;
using Microsoft.EntityFrameworkCore;

namespace TasksBll.Implments
{
    public partial class UserLoginService : BaseService<UserModel>, IUserLoginService
    {
        private IUserLoginDAL _userLoginDAL;
        public UserLoginService(IUserLoginDAL userLoginDAL)
        {
            _userLoginDAL = userLoginDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _userLoginDAL;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public UserModel Login(string name, string par)
        {
            return Dal.FindList<UserModel>(a => a.UserName == name && a.PassWord == par).FirstOrDefault();
        }
        /// <summary>
        /// 获取任务运行和未运行的数量
        /// </summary>
        /// <returns></returns>
        public int[] TaskNum()
        {
            int[] num = new int[2];
            //当前总任务数量
            var enableTask = 0;
            var disableTask = 0;
            var task = Dal.FindList<TaskModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).ToList();
            foreach (var taskModel in task)
            {
                if (taskModel.Status == 1)
                {
                    enableTask++;
                }
                else
                {
                    disableTask++;
                }
            }
            num[0] = enableTask;
            num[1] = disableTask;
            return num;
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        public object GetChartList()
        {
            var montyStartDay = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            var montyLastDay = montyStartDay.AddMonths(1).AddDays(-1);
            var log = Dal.FindList<LoggerModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).ToList();
            var task = Dal.FindList<TaskModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(a => a.CreatedTime).ToList();
            var query = from loggerModel in log
                        join taskModel in task on loggerModel.TaskId equals taskModel.Id into t1
                        from t1Model in t1.DefaultIfEmpty()
                        select new
                        {
                            runTime = loggerModel.RunTime,
                            runStatus = loggerModel.Status,
                            createdTime = loggerModel.CreatedTime,
                            name = t1Model.Name,
                            taskId = loggerModel.TaskId
                        };
            return query.Where(rs => rs.createdTime >= montyStartDay && rs.createdTime <= montyLastDay)
             .ToArray();
        }
        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        public List<TaskInfoEntity> GetTaskList()
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
                            UserEmail = t1Model.Email,
                            TriggerId = taskModel.TriggerId,
                            TriggerValue = taskModel.TriggerValue,
                            TriggerDesc = taskModel.TriggerDesc,
                            Type = taskModel.Type,
                            Value = taskModel.Value,
                            Status = taskModel.Status,
                            CreatedTime = taskModel.CreatedTime,
                            GroupName = t2Model.Name
                        };
            return query.ToList();
        }

    }
}
