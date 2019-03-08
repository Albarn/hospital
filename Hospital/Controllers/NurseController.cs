using Hospital.DataAccess;
using Hospital.DataAccess.EntityFramework;
using Hospital.DataAccess.Models;
using Hospital.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    public class NurseController : Controller
    {
        private IRepository<Nurse> nurses = new NurseRepository();

        // GET: Nurse
        public ActionResult Index()
        {
            return View(nurses.GetAll().OrderBy(d => d.FullName));
        }

        private bool IsUserNurse(string id)
        {
            return HttpContext
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindById(id)
                .IsInRole(Role.Nurse);
        }
    }
}