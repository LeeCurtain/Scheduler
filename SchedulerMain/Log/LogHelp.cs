using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SchedulerMain.Log
{
    public class LogHelp
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        static public string LOG_PATH = @"/mnt/logs/ZDSyncService";
        /// <summary>
        /// 日志名称
        /// </summary>
        static public string LOG_NAME { get; set; }
        public LogHelp(string LogName = "")
        {
            if (!Directory.Exists(LOG_PATH))
            {
                Directory.CreateDirectory(LOG_PATH);
            }
            LOG_NAME = LogName;
        }
        /// <summary>
        /// 正常记录信息
        /// </summary>
        /// <param name="msg"></param>
        public void info(string msg)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(LOG_PATH + @"/" + LOG_NAME + DateTime.Now.ToString("yyyyMMdd") + ".log", true, Encoding.UTF8);
                w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + " [INFO ] : " + msg);
            }
            catch
            {
            }
            finally
            {
                if (w != null) w.Close();
            }
        }
        /// <summary>
        /// 提醒信息
        /// </summary>
        /// <param name="msg"></param>
        public void warn(string msg)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(LOG_PATH + @"/" + LOG_NAME + DateTime.Now.ToString("yyyyMMdd") + ".log", true, Encoding.UTF8);
                w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + " [WARN] : " + msg);
            }
            catch
            {
            }
            finally
            {
                if (w != null) w.Close();
            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="msg"></param>
        public void error(string msg)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(LOG_PATH + @"/" + LOG_NAME + DateTime.Now.ToString("yyyyMMdd") + ".log", true, Encoding.UTF8);
                w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + " [ERROR] : " + msg);
            }
            catch
            {
            }
            finally
            {
                if (w != null) w.Close();
            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex"></param>
        public void exception(Exception ex)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(LOG_PATH + @"/" + LOG_NAME + DateTime.Now.ToString("yyyyMMdd") + ".log", true, Encoding.UTF8);
                w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + " [ERROR] : " + ex.Message);
                w.WriteLine(ex.StackTrace);
            }
            catch
            {
            }
            finally
            {
                if (w != null) w.Close();
            }
        }
    }
}
