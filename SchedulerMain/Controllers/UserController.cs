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
    public class UserController : BaseController
    {
        private IConfiguration _configuration;
        private readonly IUserService _userService;
        public UserController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }
        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.Normal)]
        public IActionResult Info()
        {
            var result = _userService.Info();
            if (result == null)
                ViewBag.UserInfo = result;
            return View();
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Normal)]
        public JsonResult InfoDo()
        {
            var passWord = GetParam<string>("inputPass");
            var email = GetParam<string>("inputEmail");
            var mobile = GetParam<string>("inputMobile");
            Request.Validation(new ValidationEntity { Key = email, Des = "邮箱格式不正在确", Type = ValidationType.Email });
            var result = _userService.InfoDo(passWord, email, mobile);
            ClearAuth();
            return result;
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult GetList()
        {
            var page = GetParam<int>("page", 1);
            return _userService.GetList(page, PageNum);
        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult AddDo()
        {
            var userName = GetParam<string>("inputName");
            var passWord = GetParam<string>("inputPass");
            var email = GetParam<string>("inputEmail");
            var mobile = GetParam<string>("inputMobile");
            var status = GetParam<string>("inputStatus") == "on" ? 1 : 0;
            var level = GetParam<int>("selectLevel") == 2 ? 2 : 1;
            Request.Validation(
                new ValidationEntity { Key = userName, Des = "用户名不得为空", Type = ValidationType.Required },
                new ValidationEntity { Key = passWord, Des = "密码不得为空", Type = ValidationType.Required },
                new ValidationEntity { Key = email, Des = "邮箱格式不正在确", Type = ValidationType.Email }
                );
            return _userService.AddDo(userName, passWord, email, mobile, status, level);

        }

        [HttpGet]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public IActionResult Edit()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            var result = _userService.Edit(id);
            if (result == null)
            {
                return Tools.ReJson("获取用户信息失败");
            }
            ViewBag.UserInfo = result;
            return View();
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult EditDo()
        {
            var id = GetParam<int>("inputId");
            var userName = GetParam<string>("inputName");
            var passWord = GetParam<string>("inputPass");
            var email = GetParam<string>("inputEmail");
            var mobile = GetParam<string>("inputMobile");
            var status = GetParam<string>("inputStatus") == "on" ? 1 : 0;
            var level = GetParam<int>("selectLevel") == 2 ? 2 : 1;
            Request.Validation(
                new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt },
                new ValidationEntity { Key = userName, Des = "用户名不得为空", Type = ValidationType.Required },
                new ValidationEntity { Key = email, Des = "邮箱格式不正在确", Type = ValidationType.Email }
            );
            var result = _userService.EditDo(id, userName, passWord, email, mobile, status, level);
            //如果是本人信息，则清空登录信息需要重新登录
            if (Auth.Info.Id == id)
            {
                ClearAuth();
            }
            return result;
        }

        [HttpPost]
        [Auth(AllowLevel = IdentityLevel.Admin)]
        public JsonResult DelDo()
        {
            var id = GetParam<int>("id");
            Request.Validation(new ValidationEntity { Key = id, Des = "ID不可小于零", Type = ValidationType.IdInt });
            if (id == Auth.Info.Id)
            {
                return Tools.ReJson("不可以删除自已");
            }
            return _userService.DelDo(id);
        }

    }
}