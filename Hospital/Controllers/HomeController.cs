using Hospital.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    [ExceptionHandlerAttribute]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}