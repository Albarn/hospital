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
    public class DoctorController : Controller
    {
        private IRepository<Doctor> doctors = new DoctorRepository();

        // GET: Admin/Doctor
        public ActionResult Index()
        {
            return View(doctors.GetAll().OrderBy(d=>d.FullName));
        }

        private bool IsUserDoctor(string id)
        {
            return HttpContext
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindById(id)
                .IsInRole(Role.Doctor);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult New(string id)
        {
            if (!IsUserDoctor(id)) return HttpNotFound();

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(string id, CreateDoctorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!IsUserDoctor(id)) return HttpNotFound();

            var doctor = new Doctor()
            {
                UserId = id,
                FullName = model.FullName,
                Position = model.Position
            };
            doctors.Add(doctor);
            return RedirectToAction("Index");
        }

        public ActionResult Categorized()
        {
            return View(doctors.GetAll().OrderBy(d => d.FullName));
        }
    }
}