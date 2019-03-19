using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Filters
{
    /// <summary>
    /// handles exceptions and writes them to log
    /// </summary>
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //get logger instance and write exception with it's stacktrace
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(filterContext.Exception.Message+"/"+filterContext.Exception.StackTrace);

            //handle exception
            var result = new ViewResult()
            {
                ViewName = "Error"
            };
            filterContext.Result=result;
            filterContext.ExceptionHandled = true;
        }
    }
}