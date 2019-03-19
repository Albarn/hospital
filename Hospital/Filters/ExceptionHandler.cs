using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Filters
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(filterContext.Exception.Message+"/"+filterContext.Exception.StackTrace);
            var result = new ViewResult()
            {
                ViewName = "Error"
            };
            filterContext.Result=result;
            filterContext.ExceptionHandled = true;
        }
    }
}