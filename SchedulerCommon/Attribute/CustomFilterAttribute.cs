using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SchedulerCommon.Attribute
{
    public class CustomFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Ons the action executing.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isDefined = false;
            var isDefinedController = false;
            var controllerActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {

                isDefinedController = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
            }
            if (isDefined || isDefinedController) return;

            string jwt = filterContext.HttpContext.Request.Headers.Where(a => a.Key == "token").Select(b => b.Value).FirstOrDefault();
            if (jwt != "FE85E814FD656A2D490B842C6D33019D")
            {
                filterContext.Result = new UnauthorizedErrorResult(new ErrorResponse("操作未授权"));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    /// <summary>
    /// Unauthorized error result.
    /// </summary>
    public class UnauthorizedErrorResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <param name="value">Value.</param>
        public UnauthorizedErrorResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }

    /// <summary>
    /// Error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="msg">Message.</param>
        public ErrorResponse(string msg)
        {
            Message = msg;
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
        //public object DeveloperMessage { get; set; }
    }
}
