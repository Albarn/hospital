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
    [ExceptionHandler]
    [RequestLogger]
    public class DoctorController : Controller
    {
        private IRepository<Doctor> doctors;
        private IRepository<Treatment> treatments;

        public DoctorController(IRepository<Doctor> doctors, IRepository<Treatment> treatments)
        {
            this.doctors = doctors;
            this.treatments = treatments;
        }
        
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

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                if (User.IsInRole(Role.Doctor.ToString())) id = UserService.GetUserId();
                else return HttpNotFound();
            }
            Doctor doctor = doctors.Find(id);
            if (doctor != null)
                return View(doctors.Find(id));
            else
                return HttpNotFound();
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
            UserService.SetUserConfirmed(id);
            return RedirectToAction("Index");
        }
    }
}