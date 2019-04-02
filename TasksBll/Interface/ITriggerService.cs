using Microsoft.AspNetCore.Mvc;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksBll.Interface
{
    public interface ITriggerService
    {
        /// <summary>
        /// 获取所有的触发器
        /// </summary>
        /// <returns></returns>
         JsonResult GetList(int page, int PageNum);
        /// <summary>
        /// 添加触发器
        /// </summary>
        /// <returns></returns>
        JsonResult AddDo(string name, string value);
        /// <summary>
        /// 获取触发器信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TriggerModel Edit(int id);
        /// <summary>
        /// 修改触发器
        /// </summary>
        /// <returns></returns>
        JsonResult EditDo(int id, string name, string value);
        /// <summary>
        /// 删除触发器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResult DelDo(int id);
    }
}
