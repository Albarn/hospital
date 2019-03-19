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
    [Authorize]
    [ExceptionHandler]
    public class PatientController : Controller
    {
        private IRepository<Patient> patients;

        public PatientController(IRepository<Patient> patients)
        {
            this.patients = patients;
        }

        // GET: Patient
        [Authorize(Roles = "Admin, Doctor, Nurse")]
        public ActionResult Index()
        {
            return View(patients.GetAll());
        }

        [Authorize(Roles="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();

            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult New(CreatePatientViewModel model,string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            //check if patient age is correct
            DateTime minDate = DateTime.Now.AddYears(-18),
                maxDate = DateTime.Now.AddYears(-5);
            if (model.BirthDate<minDate || model.BirthDate > maxDate)
            {
                ModelState.AddModelError("BirthDate", "Invalid Birth Date, patient must be from 5 to 18 years old.");
                return View(model);
            }

            Patient patient = new Patient()
            {
                BirthDate = model.BirthDate,
                FullName = model.FullName,
                UserId = id
            };
            patients.Add(patient);
            UserService.SetUserConfirmed(id);
            return RedirectToAction("Index");
        }
        
        public ActionResult Details(string id)
        {
            if (User.IsInRole("Patient")) id = UserService.GetUserId();
            else if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();

            var patient = patients.Find(id);
            if (patient == null) return HttpNotFound();
            return View(patient);
        }
    }
}