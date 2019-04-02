using Microsoft.AspNetCore.Mvc;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksBll.Interface
{
    public interface IUserService
    {
        /// <summary>
        /// 获取登录人信息
        /// </summary>
        /// <returns></returns>
         UserModel Info();
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <returns></returns>
        JsonResult InfoDo(string passWord, string email, string mobile);
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        JsonResult GetList(int page, int PageNum);
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        JsonResult AddDo(string userName, string passWord, string email, string mobile, int status, int level);
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <returns></returns>
        UserModel Edit(int id);
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <returns></returns>
         JsonResult EditDo(int id, string userName, string passWord, string email, string mobile, int status, int level);

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <returns></returns>
        JsonResult DelDo(int id);
    }
}
