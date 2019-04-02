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
using TasksBll.Interface;
using TasksEntity.Model;
namespace SchedulerMain.Controllers
{
    [Auth(AllowLevel = IdentityLevel.Admin)]
    public class GroupController : BaseController
    {
        private IConfiguration _configuration;
        private readonly IGroupService _groupService;
        public GroupController(IConfiguration configuration, IGroupService groupService)
        {
            _configuration = configuration;
            _groupService = groupService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.GroupList = _groupService.GetList();
            return View();
        }

        [HttpPost]
        public JsonResult AddDo()
        {
            var name = GetParam<string>("name");
            Request.Validation(new ValidationEntity { Key = name, Des = "任务组名称不可为空", Type = ValidationType.Required });
            return _groupService.AddDo(name);
        }

        [HttpPost]
        public JsonResult EditDo()
        {
            var id = GetParam<int>("id");
            var name = GetParam<string>("name");
            Request.Validation(
                new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt },
                new ValidationEntity { Key = name, Des = "任务组名称不可为空", Type = ValidationType.Required });
            return _groupService.EditDo(id, name);
        }

        [HttpPost]
        public JsonResult DelDo()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            return _groupService.DelDo(id);
        }
    }
}