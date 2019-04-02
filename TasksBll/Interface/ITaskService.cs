using Microsoft.AspNetCore.Mvc;
using SchedulerCommon.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksEntity.BM_Models;

namespace TasksBll.Interface
{
    public interface ITaskService
    {
        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNum"></param>
        TaskList TaskListFunc(int page, int PageNum);
        /// <summary>
        /// 任务的下拉列表
        /// </summary>
        /// <returns></returns>
        TaskTG TaskAddTG();
        /// <summary>
        /// 编辑任务获取信息
        /// </summary>
        /// <returns></returns>
        TaskTTG TaskeditTTG(int id);
        /// <summary>
        /// 新增加任务
        /// </summary>
        /// <returns></returns>
        JsonResult AddDo(string name, string desc, string type, string value, string triggerId, string groupId);
        /// <summary>
        /// 修改任务
        /// </summary>
        /// <returns></returns>
        JsonResult EditDo(int id, string name, string desc, int type, string value, int triggerId, int groupId);
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        JsonResult DelDo(int id);
        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        JsonResult StatusDo(int id, int status);
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <returns></returns>
        JsonResult RunDo(int id);
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        JsonResult PauseDo(string id, int taskId);

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <returns></returns>
        JsonResult ResumeDo(string id, int taskId);

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        JsonResult GetLogger(int id, int page, int PageNum);
    }
}
