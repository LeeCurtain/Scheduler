using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TasksCommon
{
    public class Functions
    {
        public static string GetTimeSpan(DateTime bdate, DateTime edate)
        {
            TimeSpan ts = edate.Subtract(bdate);

            int c = 60;
            int d = 60 * c;
            int e = 24 * d;
            int f = 7 * e;

            if (c > ts.TotalSeconds)
            { return "刚刚"; }
            else if (d > ts.TotalSeconds)
            { return ts.Minutes.ToString() + "分钟前"; }
            else if (e > ts.TotalSeconds)
            { return ts.Hours.ToString() + "小时前"; }
            else if (f > ts.TotalSeconds)
            { return ts.Days.ToString() + "天前"; }
            else { return (ts.Days / 7).ToString() + "周前"; }
        }


        public static string GetVisitTimeSpan(DateTime bdate, DateTime edate)
        {
            TimeSpan ts = edate.Subtract(bdate);

            int c = 60;
            int d = 60 * c;
            int e = 24 * d;
            int f = 7 * e;
            int g = 30 * e;

            if (c > ts.TotalSeconds)
            { return "刚刚"; }
            else if (d > ts.TotalSeconds)
            { return ts.Minutes.ToString() + "分钟前"; }
            else if (e > ts.TotalSeconds)
            { return ts.Hours.ToString() + "小时前"; }
            else if (f > ts.TotalSeconds)
            { return ts.Days.ToString() + "天前"; }
            else if (g > ts.TotalSeconds)
            { return (ts.Days / 7).ToString() + "周前"; }
            else { return (ts.Days / 30).ToString() + "月前"; }
        }

        public string GetFileJson(string filepath)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312")))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return json;
        }

        /// <summary>
        ///  重写比较对象所有不为空的属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">要比较的原对象</param>
        /// <param name="equalObj">被比较的对象</param>
        /// <returns></returns>
        public static bool Equals<T>(T t, T equalObj)
        {
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (null == pi.GetValue(equalObj, null))
                    continue;
                if (pi.PropertyType.Module.Name == "System.Data.Entity.dll")
                    continue;

                if (!pi.GetValue(equalObj, null).Equals(pi.GetValue(t, null)))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 修改属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">数据库中待修改的对象(从数据库中获取出来的完整对象)</param>
        /// <param name="changT">要被保存的对象</param>
        public static void SetModelValue<T>(T t, T changT) where T : new()
        {
            try
            {
                foreach (PropertyInfo pi in typeof(T).GetProperties())
                {
                    if (pi.GetValue(changT, null) == null)
                        continue;
                    if (pi.PropertyType.Module.Name == "System.Data.Entity.dll")
                        continue;
                    pi.SetValue(t, pi.GetValue(changT, null), null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("在给{0}的对象赋值时出错,出错信息：{1}", t.GetType().Name, ex.Message));
            }
        }
    }
}
