using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchedulerCommon.Entity;
using SchedulerCommon.Enum;
using SchedulerCommon.Library;
using SchedulerModel.Entity;
using TasksBll.Interface;
using TasksEntity.Model;

namespace SchedulerMain.Controllers
{
    public class LoginController : BaseController
    {
        private IConfiguration _configuration;
        private IUserLoginService _userLoginService;
        public LoginController(IConfiguration configuration, IUserLoginService userLoginService)
        {
            _configuration = configuration;
            _userLoginService = userLoginService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Do()
        {
            var cmd = GetParam<int>("cmd");
            if (cmd == 0)
            {
                ClearAuth();
            }
            else
            {
                var userName = GetParam<string>("inputName");
                var passWord = GetParam<string>("inputPass");
                Request.Validation(
                   new ValidationEntity { Key = userName, Des = "用户名不得为空", Type = ValidationType.Required },
                   new ValidationEntity { Key = passWord, Des = "用户密码不得为空", Type = ValidationType.Required }
               );
                var newPass = Tools.Md5(passWord);
                var result = _userLoginService.Login(userName, newPass); if (result == null)
                {
                    return Tools.ReJson("获取用户信息失败");
                }

                if (result.Status == 0)
                {
                    return Tools.ReJson("该用户已被禁用");
                }

                HttpContext.Session.SetString(Auth.Key, JsonConvert.SerializeObject(new AuthInfoEntity()
                {
                    Id = result.Id,
                    UserName = result.UserName,
                    Email = result.Email,
                    Level = result.Level,
                    Mobile = result.Mobile,
                    Status = result.Status,
                }));
                // }
            }

            return Tools.ReJson();
        }
    }
}