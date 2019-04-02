using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;
using Quartz;
using SchedulerCommon.Entity;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TasksEntity.Model;
namespace SchedulerQuartz
{
    public class ScheduleJob : IJob
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        private async Task InsertLogger(LoggerModel logger)
        {
            //using (var db = new SqlContext())
            //{
            //    await db.Logger.AddAsync(logger);
            //    db.SaveChanges();
            //}
            WriteService writeService = new WriteService();
            await writeService.InsertLogger(logger);
        }

        /// <summary>
        /// 执行命令行程序 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static KeyValuePair<string, bool> RunConsole(string value)
        {
            var result = "";
            var status = true;
            try
            {
                var proc = Process.Start(new ProcessStartInfo(value) { RedirectStandardOutput = true });
                if (proc != null)
                {
                    using (var str = proc.StandardOutput)
                    {
                        result += str.ReadToEnd();
                    }

                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                result += e.Message;
                status = false;
            }

            return new KeyValuePair<string, bool>(result, status);
        }       
        /// <summary>
        /// 执行CURL请求程序
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static KeyValuePair<string, bool> RunCurl(string value)
        {
            var result = "";
            var status = true;
            try
            {
                result = SendRequest(value, Encoding.UTF8);
                //var webClient = new WebClient { Encoding = Encoding.UTF8 };
                //webClient.Headers.Add("token", "FE85E814FD656A2D490B842C6D33019D");
                //var rs = webClient.OpenReadTaskAsync(value).Result;
                //Console.WriteLine(rs);
                //if (rs != null)
                //{
                //    using (var str = new StreamReader(rs, Encoding.UTF8))
                //    {
                //        result += str.ReadToEnd();
                //        Console.WriteLine(str.Peek());
                //    }
                //}

            }
            catch (Exception e)
            {
                result += e.Message;
                status = false;
            }

            return new KeyValuePair<string, bool>(result, status);
        }



        /// <summary>
        /// 发送WebSocket消息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task SendMessage(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;
            if (!(jobData.Get("SchedulerHub") is SchedulerSignalR.SchedulerExtHub<SchedulerSignalR.SchedulerHub> schedulerHub))
            {
                return;
            }

            var message = new JObject
            {
                ["type"] = 1,
                ["jobKey"] = context.JobDetail.Key.Name,
                ["triggerKey"] = context.Trigger.Key.Name,
                ["triggerPreTime"] = context.Trigger.GetPreviousFireTimeUtc().HasValue
                    ? context.Trigger.GetPreviousFireTimeUtc().GetValueOrDefault().LocalDateTime
                        .ToString("yyyy-MM-dd HH:mm:ss")
                    : "",
                ["triggerNextTime"] = context.Trigger.GetNextFireTimeUtc().HasValue
                    ? context.Trigger.GetNextFireTimeUtc().GetValueOrDefault().LocalDateTime
                        .ToString("yyyy-MM-dd HH:mm:ss")
                    : "",
            };
            await schedulerHub.SendMessage(message.ToString());
        }

        /// <summary>
        /// 执行http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String SendRequest(String url, Encoding encoding)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("token", "FE85E814FD656A2D490B842C6D33019D");
            webRequest.Method = "GET";
            webRequest.Timeout = 8 * 60 * 1000;//八分钟
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);

            //StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding(encoding));

            return sr.ReadToEnd();
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailInfo"></param>
        /// <param name="taskInfo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static async Task SendEmail(EmailEntity emailInfo, TaskInfoEntity taskInfo, string result, string source)
        {
            string[] arry = taskInfo.UserEmail.Split(',');
            foreach (string str in arry)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailInfo.User));
                message.To.Add(new MailboxAddress(taskInfo.UserName, str));
                message.Subject = $"报警邮件，任务：{taskInfo.Name}" + "  出错环境：" + source;
                var body = new BodyBuilder { HtmlBody = result };
                message.Body = body.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                    client.Connect(emailInfo.Smtp, emailInfo.Port, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailInfo.User, emailInfo.Pass);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }


        }


        /// <summary>
        /// 执行Job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;
            if (!(jobData.Get("EmailInfo") is EmailEntity emailInfo))
            {
                return;
            }
            if (!(jobData.Get("TaskInfo") is TaskInfoEntity taskInfo))
            {
                return;
            }

            //根据job运行类型执行相应的方法，返回运行的结果和状态
            var sw = new Stopwatch();
            sw.Start();
            var run = taskInfo.Type == 1 ? RunConsole(taskInfo.Value) : RunCurl(taskInfo.Value);
            sw.Stop();
            var result = run.Key;
            //修改变量为强类型
            bool status = run.Value;
            if (!status)
            {
                //新增服务器名
                result += ",服务器名"+ServerName();
                string source = "Product";
                if (taskInfo.Name != "first" && (taskInfo.Value.Contains("zddemo")|| taskInfo.Value.Contains("proadmin")))
                {
                    if (taskInfo.Value.Contains("zddemo"))
                        source = "Demo";                   
                    //发送邮件消息
                    await SendEmail(emailInfo, taskInfo, result, source);
                }
            }
            //判断执行任务是否为防止进程死掉的任务
            if (taskInfo.Name != "first")
            {
                await InsertLogger(new LoggerModel
                {
                    TaskId = taskInfo.Id,
                    Result = result,
                    Status = status ? 1 : 0,
                    RunTime = decimal.Parse(sw.Elapsed.TotalSeconds.ToString()),
                    CreatedTime = DateTime.Now.ToLocalTime(),
                    UpdatedTime = DateTime.Now.ToLocalTime()
                });
            }

            //发送WebSocket消息
            //await SendMessage(context);
        }

        /// <summary>
        /// 获取服务名的方法
        /// </summary>
        /// <returns></returns>
        public string ServerName()
        {
            var psi = new System.Diagnostics.ProcessStartInfo("hostname");
            psi.RedirectStandardOutput = true;
            using (var process = System.Diagnostics.Process.Start(psi))
            {
                var computerName = process.StandardOutput.ReadToEnd();
                return computerName;
            }
        }
    }
}
