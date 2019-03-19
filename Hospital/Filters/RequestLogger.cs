using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Filters
{
    /// <summary>
    /// writes request url and method to log
    /// </summary>
    public class RequestLoggerAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //write with given HttpContext
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(filterContext.HttpContext.Request.HttpMethod + "/" + filterContext.HttpContext.Request.Url);
        }
    }
}