using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.Models;
using Hospital.Filters;
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
    [ExceptionHandlerAttribute]
    [RequestLogger]
    public class NurseController : Controller
    {
        private IRepository<Nurse> nurses;

        public NurseController(IRepository<Nurse> nurses)
        {
            this.nurses = nurses;
        }

        // GET: Nurse
        public ActionResult Index()
        {
            return View(nurses.GetAll().OrderBy(d => d.FullName));
        }

        public ActionResult Details(string id)
        {
            if (id == null && User.IsInRole(Role.Nurse.ToString())) id = UserService.GetUserId();
            Nurse nurse = nurses.Find(id);
            if (nurse == null) return HttpNotFound();
            else return View(nurse);
        }

        [Authorize(Roles="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Nurse)) return HttpNotFound();

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
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
            UserService.SetUserConfirmed(id);
            return RedirectToAction("Index");
        }
    }
}