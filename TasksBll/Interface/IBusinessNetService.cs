using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TasksEntity.Model;

namespace TasksBll.Interface
{
    public interface IBusinessNetService
    {
        string BusinessNetWorkInfo();

        /// <summary>
        /// 生意网络统计
        /// </summary>
        /// <returns></returns>
        string BusinessNet();
    }
}
