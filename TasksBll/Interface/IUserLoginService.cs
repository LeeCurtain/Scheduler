using SchedulerCommon.Entity;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksBll.Interface
{
    public  interface IUserLoginService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="par"></param>
        /// <returns></returns>
         UserModel Login(string name, string par);
        /// <summary>
        /// 获取任务运行和未运行的数量
        /// </summary>
        /// <returns></returns>
        int[] TaskNum();
        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        object GetChartList();
        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
         List<TaskInfoEntity> GetTaskList();
    }
}
