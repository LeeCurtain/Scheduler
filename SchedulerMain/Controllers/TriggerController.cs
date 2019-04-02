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
    [Auth(AllowLevel = IdentityLevel.All)]
    public class TriggerController : BaseController
    {
        private IConfiguration _configuration;
        private readonly ITriggerService _triggerService;
        public TriggerController(IConfiguration configuration, ITriggerService triggerService)
        {
            _configuration = configuration;
            _triggerService = triggerService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetList()
        {
            var page = GetParam<int>("page", 1);
            return _triggerService.GetList(page, PageNum);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddDo()
        {
            var name = GetParam<string>("inputName");
            var value = GetParam<string>("inputExpCron");
            Request.Validation(
                new ValidationEntity { Key = name, Des = "触发器名称不得为空", Type = ValidationType.Required },
                new ValidationEntity { Key = value, Des = "触发器规则不得为空", Type = ValidationType.Required }
            );
            return _triggerService.AddDo(name, value);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            var result = _triggerService.Edit(id);
            if (result == null)
            {
                return Tools.ReJson("获取触发器信息失败");
            }
            ViewBag.TriggerInfo = result;
            return View();
        }

        [HttpPost]
        public JsonResult EditDo()
        {
            var id = GetParam<int>("inputId");
            var name = GetParam<string>("inputName");
            var value = GetParam<string>("inputExpCron");
            Request.Validation(
                new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt },
                new ValidationEntity { Key = name, Des = "触发器名称不得为空", Type = ValidationType.Required },
                new ValidationEntity { Key = value, Des = "触发器规则不得为空", Type = ValidationType.Required }
            );
            return _triggerService.EditDo(id, name, value);
        }

        [HttpPost]
        public JsonResult DelDo()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            return _triggerService.DelDo(id);
        }

        [HttpGet]
        public JsonResult TestRule()
        {
            var rule = GetParam<string>("rule");
            var count = GetParam<int>("count", 1);
            var list = SchedulerServer.FireTimeList(rule, count);
            return list.Count <= 0 ? Tools.ReJson("触发器规则不正确") : Tools.ReJson(list);
        }
    }
}