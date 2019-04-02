using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SchedulerMain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        /// <summary> 
        /// proadmin.9wins.cn 生产  iiscert demo
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5006")
                //.UseKestrel(options => options.Listen(IPAddress.Any, 5006, listenOptions =>
                //{

                //    listenOptions.UseHttps(new X509Certificate2("/home/cert/proadmin/proadmin.9wins.cn.pfx", "9Wins123456"));
                //}))
                 .UseStartup<Startup>();
    }
}
