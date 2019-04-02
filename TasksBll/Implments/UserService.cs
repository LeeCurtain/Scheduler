using SchedulerModel.Entity;
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
    public partial class UserService : BaseService<UserModel>, IUserService
    {
        private IUserDAL _userDAL;
        public UserService(IUserDAL userDAL)
        {
            _userDAL = userDAL;
            SetDal();
        }
        public override void SetDal()
        {
            Dal = _userDAL;
        }
        /// <summary>
        /// 获取登录人信息
        /// </summary>
        /// <returns></returns>
        public UserModel Info()
        {
            var result = Dal.FindList<UserModel>(rs => rs.Id == Auth.Info.Id).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult InfoDo(string passWord, string email, string mobile)
        {
            var result = Dal.FindList<UserModel>(rs => rs.Id == Auth.Info.Id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取用户信息失败");
            }
            if (!string.IsNullOrWhiteSpace(passWord))
            {
                result.PassWord = Tools.Md5(passWord);
            }
            result.Email = email;
            result.Mobile = mobile;
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("更新用户信息失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList(int page, int PageNum)
        {
            var result = Dal.FindList<UserModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).OrderByDescending(rs => rs.Level).ThenByDescending(rs => rs.Id).Skip(PageNum * (page - 1))
                .Take(PageNum).ToList();
            return Tools.ReJson(new
            {
                list = result,
                total = Dal.FindList<UserModel>(a => DateTime.Parse(a.CreatedTime.ToString()) <= DateTime.Now).Count(),
                pageNum = PageNum,
                currentPage = page
            });
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public JsonResult AddDo(string userName, string passWord, string email, string mobile, int status, int level)
        {
            if (Dal.FindList<UserModel>(rs => rs.UserName == userName).AsNoTracking().Any())
            {
                return Tools.ReJson("该用户名已存在");
            }
            UserModel userModel = new UserModel()
            {
                UserName = userName,
                PassWord = Tools.Md5(passWord),
                Level = level,
                Mobile = mobile,
                Email = email,
                Status = status,
                CreatedTime = DateTime.Now.ToLocalTime(),
                UpdatedTime = DateTime.Now.ToLocalTime()
            };
            Dal.Add(userModel);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("添加用户失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <returns></returns>
        public UserModel Edit(int id)
        {
            var result = Dal.FindList<UserModel>(rs => rs.Id == id).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult EditDo(int id, string userName, string passWord, string email, string mobile, int status, int level)
        {
            if (Dal.FindList<UserModel>(rs => rs.UserName == userName && rs.Id != id).AsNoTracking().Any())
            {
                return Tools.ReJson("该用户名已存在");
            }
            var result = Dal.FindList<UserModel>(rs => rs.Id == id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取用户信息失败");
            }
            if (!string.IsNullOrWhiteSpace(passWord))
            {
                result.PassWord = Tools.Md5(passWord);
            }
            result.Email = email;
            result.Mobile = mobile;
            result.UserName = userName;
            result.Level = level;
            result.Status = status;
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("更新用户信息失败");
            }
            return Tools.ReJson();
        }
        /// <summary>
        /// 删除信息
        /// </summary>
        /// <returns></returns>
        public JsonResult DelDo(int id)
        {
            var result = Dal.FindList<UserModel>(rs => rs.Id == id).FirstOrDefault();
            if (result == null)
            {
                return Tools.ReJson("获取用户信息失败");
            }
            Dal.Delete(result);
            if (!Dal.SaveChanges())
            {
                return Tools.ReJson("删除用户信息失败");
            }
            return Tools.ReJson();
        }
    }
}
