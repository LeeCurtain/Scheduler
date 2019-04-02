using Microsoft.AspNetCore.Mvc.Filters;
using SchedulerCommon.Factory;
using SchedulerCommon.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCommon.Filter
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 200;
            if (context.Exception is UtilException exception)
            {
                context.Result = Tools.ReJson(exception.Msg);
                Console.WriteLine(exception.Msg);
            }
        }
    }
}

