using Microsoft.AspNetCore.Mvc;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksBll.Interface
{
    public interface IGroupService
    {
        /// <summary>
        /// 获取所有分组
        /// </summary>
        /// <returns></returns>
        List<GroupModel> GetList();
        /// <summary>
        /// 新增分组
        /// </summary>
        /// <returns></returns>
        JsonResult AddDo(string name);

        /// <summary>
        /// 修改组名
        /// </summary>
        /// <returns></returns>
        JsonResult EditDo(int id, string name);
        /// <summary>
        /// 删除组
        /// </summary>
        /// <returns></returns>
        JsonResult DelDo(int id);
    }
}
