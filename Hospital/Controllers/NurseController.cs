using Hospital.BLL;
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

        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Nurse)) return HttpNotFound();

            return View();
        }

        [HttpPost]
        public ActionResult New(CreateNurseViewModel model, string id)
        {
            if (!UserService.IsUserInRole(id, Role.Nurse)) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            var nurse = new Nurse()
            {
                FullName = model.FullName,
                UserId = id
            };
            nurses.Add(nurse);
            return RedirectToAction("Index");
        }
    }
}