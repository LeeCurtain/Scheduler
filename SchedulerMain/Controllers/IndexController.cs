using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchedulerCommon.Attribute;
using SchedulerCommon.Enum;
using SchedulerCommon.Library;
using SchedulerModel.Entity;
using SchedulerQuartz;
using TasksBll.Interface;
using TasksEntity.Model;

namespace SchedulerMain.Controllers
{
    [Auth(AllowLevel = IdentityLevel.All)]
    public class IndexController : BaseController
    {
        private readonly SchedulerServer _schedulerServer;
        private IConfiguration _configuration;
        private IUserLoginService _userLoginService;
        public IndexController(SchedulerServer schedulerServer, IConfiguration configuration, IUserLoginService userLoginService)
        {
            _schedulerServer = schedulerServer;
            _configuration = configuration;
            _userLoginService = userLoginService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            int[] num = _userLoginService.TaskNum();
            ViewBag.EnableTask = num[0];
            ViewBag.DisableTask = num[1];
            //运行中的任务
            var triggerResult = _schedulerServer.GetJobCount();
            ViewBag.TaskRunCount = triggerResult.Value;
            ViewBag.TaskTotalCount = triggerResult.Key;
            return View();
        }

        /// <summary>
        /// 获取job列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetJobList()
        {
            return Tools.ReJson(_schedulerServer.GetJobList());
        }

        /// <summary>
        /// 获取一些统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetChartList()
        {
            return Tools.ReJson(_userLoginService.GetChartList());
        }
    }
}

