using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TasksDAL.Interface;

namespace TasksBll.Implments
{
    public delegate object ExecuteServiceCallBackHandler();
    public abstract partial class BaseService<T> where T : class, new()
    {

        public IBaseDAL<T> Dal { get; set; }

        public abstract void SetDal();

        public bool Add(T t)
        {
            Dal.Add(t);
            return Dal.SaveChanges();
        }
        public bool Delete(T t)
        {
            Dal.Delete(t);
            return Dal.SaveChanges();
        }
        public bool Update(T t)
        {
            Dal.Update(t);
            return Dal.SaveChanges();
        }
        /// <summary>
        /// 执行业务方法,建议业务层所有业务均执行该方法
        /// </summary>
        /// <param name="callBackHandler"></param>
        /// <returns></returns>
        public object ExecuteService(ExecuteServiceCallBackHandler callBackHandler)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                object ret = null;
                if (callBackHandler != null)
                {
                    ret = callBackHandler();
                    Dal.SaveChanges();
                }
                ts.Complete();
                return ret;
            }
        }
    }
}
