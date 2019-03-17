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
    public class DoctorController : Controller
    {
        private IRepository<Doctor> doctors = new DoctorRepository();
        private IRepository<Treatment> treatments = new TreatmentRepository();
        
        public ActionResult Index()
        {
            return View(PatientsCounter
                .CalculatePatientsNumber(doctors.GetAll(),treatments.GetAll())
                .OrderBy(d=>d.FullName));
        }

        public ActionResult Categorized()
        {
            return View(PatientsCounter
                .CalculatePatientsNumber(doctors.GetAll(), treatments.GetAll())
                .OrderBy(d => d.FullName));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id,Role.Doctor)) return HttpNotFound();

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(string id, CreateDoctorViewModel model)
        {
            if (!UserService.IsUserInRole(id, Role.Doctor)) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            var doctor = new Doctor()
            {
                UserId = id,
                FullName = model.FullName,
                Position = model.Position
            };
            doctors.Add(doctor);
            return RedirectToAction("Index");
        }
    }
}