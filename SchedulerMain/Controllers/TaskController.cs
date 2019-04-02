using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchedulerCommon.Attribute;
using SchedulerCommon.Entity;
using SchedulerCommon.Enum;
using SchedulerCommon.Library;
using SchedulerModel.Entity;
using SchedulerQuartz;
using TasksBll.Interface;
using TasksEntity.Model;

namespace SchedulerMain.Controllers
{
    public class TaskController : BaseController
    {
        private readonly SchedulerServer _schedulerServer;
        private IConfiguration Configuration { get; }
        private readonly ITaskService _taskService;
        public TaskController(SchedulerServer schedulerServer, IConfiguration configuration, ITaskService taskService)
        {
            _schedulerServer = schedulerServer;
            Configuration = configuration;
            _taskService = taskService;
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.All)]
        public IActionResult Index()
        {
            var page = GetParam<int>("page", 1);
            var query = _taskService.TaskListFunc(page, PageNum);
            ViewBag.TaskList = query.taskInfoEntity;
            ViewBag.TaskTotal = query.TaskTotal;
            ViewBag.PageNum = query.PageNum;
            ViewBag.CurrentPage = query.CurrentPage;
            return View();
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.All)]
        public IActionResult Add()
        {
            var query = _taskService.TaskAddTG();
            ViewBag.TriggerList = query.triggers;
            ViewBag.GroupList = query.groups;
            return View();
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult AddDo()
        {
            var name = GetParam<string>("inputName");
            var desc = GetParam<string>("inputDescription");
            var type = GetParam<int>("selectType");
            var value = GetParam<string>("inputValue");
            var triggerId = GetParam<int>("selectTrigger");
            var groupId = GetParam<int>("selectGroup");
            Request.Validation(
                new ValidationEntity { Key = name, Des = "任务名称不可为空", Type = ValidationType.Required },
                new ValidationEntity { Key = triggerId, Des = "触发器ID不可小于零", Type = ValidationType.IdInt },
                new ValidationEntity { Key = value, Des = "任务值不可为空", Type = ValidationType.Required }
            );
            return _taskService.AddDo(name, desc, type.ToString(), value, triggerId.ToString(), groupId.ToString());
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.All)]
        public IActionResult Edit()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });

            var result = _taskService.TaskeditTTG(id);
            if (result.Task == null)
            {
                return Tools.ReJson("获取任务信息失败");
            }

            ViewBag.TaskInfo = result.Task;
            ViewBag.TriggerList = result.triggers;
            ViewBag.GroupList = result.groups;
            return View();
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult EditDo()
        {
            var id = GetParam<int>("inputId");
            var name = GetParam<string>("inputName");
            var desc = GetParam<string>("inputDescription");
            var type = GetParam<int>("selectType");
            var value = GetParam<string>("inputValue");
            var triggerId = GetParam<int>("selectTrigger");
            var groupId = GetParam<int>("selectGroup");
            Request.Validation(
                new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt },
                new ValidationEntity { Key = name, Des = "任务名称不可为空", Type = ValidationType.Required },
                new ValidationEntity { Key = value, Des = "任务值不可为空", Type = ValidationType.Required }
            );
            return _taskService.EditDo(id, name, desc, type, value, triggerId, groupId);
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult DelDo()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            _taskService.DelDo(id);
            return Tools.ReJson(_schedulerServer.RemoveJob(id.ToString()));
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult StatusDo()
        {
            var id = GetParam<int>("id");
            var status = GetParam<int>("status") == 1 ? 1 : 0;

            return _taskService.StatusDo(id, status);
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult RunDo()
        {
            var id = GetParam<int>("id");

            return _taskService.RunDo(id);
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult PauseDo()
        {
            var id = GetParam<string>("id");
            var taskId = GetParam<int>("taskId");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可为空", Type = ValidationType.Required },
                new ValidationEntity { Key = taskId, Des = "任务ID不可小于零", Type = ValidationType.IdInt });
            return _taskService.PauseDo(id, taskId);
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult ResumeDo()
        {
            var id = GetParam<string>("id");
            var taskId = GetParam<int>("taskId");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可为空", Type = ValidationType.Required },
                new ValidationEntity { Key = taskId, Des = "任务ID不可小于零", Type = ValidationType.IdInt });

            return _taskService.ResumeDo(id, taskId);
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.All)]
        public IActionResult Logger()
        {
            return View();
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.All)]
        public JsonResult GetLogger()
        {
            var id = GetParam<int>("id");
            var page = GetParam<int>("page", 1);
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可为空", Type = ValidationType.Required });

            return _taskService.GetLogger(id, page, PageNum);
        }
    }
}