using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchedulerCommon.Attribute;
using SchedulerCommon.Enum;
using SchedulerMain.Log;
using TasksBll.Interface;
namespace SchedulerMain.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [CustomFilter]
    public class BllServiceController : Controller
    {
        private readonly IBusinessNetService _businessNetService;
        private readonly IRecommendService _recommendService;
        private readonly IUserArticleService _userArticleService;
        private readonly IConfiguration _configuration;
        //private readonly HttpContext _context;
        public BllServiceController(IBusinessNetService businessNetService, IRecommendService recommendService, IUserArticleService userArticleService,IConfiguration configuration)
        {
            _businessNetService = businessNetService;
            _recommendService = recommendService;
            _userArticleService = userArticleService;
            _configuration = configuration;
           //_context = accessor.HttpContext;
        }

        [HttpGet("api/V1/BllTset", Name = "Test")]
        public IActionResult Test()
        {
            string url = _configuration["ZDsEtting:url"];
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流
            return Ok("成功");
        }
        /// <summary>
        /// 生意网统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/V1/BllService",Name = "BusinessNet")]
        public IActionResult BusinessNet()
        {
            LogHelp log = new LogHelp("BusinessNetJob");
            string ip = "";//_context.Connection.RemoteIpAddress.ToString();
            try
            {
                log.info($"获取到的真实IP：{ip}");
                log.info("生意网络统计服务开始");
                _businessNetService.BusinessNet();
                log.info("生意网络统计服务成功");
                return Ok("统计成功,IP地址："+ip);
            }
            catch (Exception e)
            {
                log.error("生意网络统计服务失败，"+e.Message);
                return Ok("统计失败,IP地址：" + ip);
            }
        }
        /// <summary>
        /// 今日推荐
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/V1/Recommend", Name = "TodayRecommend")]
        public IActionResult TodayRecommend()
        {
            LogHelp log = new LogHelp("TodayRecommendJob");
            string ip = "";//_context.Connection.RemoteIpAddress.ToString();
            try
            {
                log.info($"获取到的真实IP：{ip}");
                log.info("今日推荐服务开始");
                _recommendService.TodayRemm();
                log.info("今日推荐服务成功");
                return Ok("推荐成功,IP地址：" + ip);
            }
            catch (Exception e)
            {
                log.error("今日推荐服务失败，" + e.Message);
                return Ok("推荐失败,IP地址：" + ip);
            }
        }
        /// <summary>
        /// 商友推荐
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/V1/RecommFri", Name = "RecommFriends")]
        public IActionResult RecommFriends()
        {
            LogHelp log = new LogHelp("RecommFriends");
            string ip = "";//_context.Connection.RemoteIpAddress.ToString();
            try
            {
                log.info($"获取到的真实IP：{ip}");
                log.info("商友推荐服务开始");
                _recommendService.RecommFriends();
                log.info("商友推荐服务成功");
                return Ok("推荐成功,IP地址：" + ip);
            }
            catch (Exception e)
            {
                log.error("商友推荐服务失败，" + e.Message);
                return Ok("推荐失败,IP地址：" + ip);
            }
        }
        /// <summary>
        /// 生意圈热门推荐
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/V1/Host", Name = "RecommHost")]
        public IActionResult RecommHost()
        {
            LogHelp log = new LogHelp("RecommHostJob");
            string ip = "";//_context.Connection.RemoteIpAddress.ToString();
            try
            {
                log.info($"获取到的真实IP：{ip}");
                log.info("生意圈热门推荐服务开始");
                _userArticleService.HostAricleByDay();
                log.info("生意圈热门推荐服务成功");
                return Ok("推荐成功,IP地址：" + ip);
            }
            catch (Exception e)
            {
                log.error("生意圈热门推荐服务失败，" + e.Message);
                return Ok("推荐失败,IP地址：" + ip);
            }
        }
    }
}