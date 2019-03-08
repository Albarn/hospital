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
    public class PatientController : Controller
    {
        private IRepository<Patient> patients = new PatientRepository();

        // GET: Patient
        public ActionResult Index()
        {
            return View(patients.GetAll());
        }

        private bool IsUserPatient(string id)
        {
            return HttpContext
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindById(id)
                ?.IsInRole(Role.Patient) ?? false;
        }

        [Authorize(Roles="Admin")]
        public ActionResult New(string id)
        {
            if (!IsUserPatient(id)) return HttpNotFound();

            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult New(CreatePatientViewModel model,string id)
        {
            if (!IsUserPatient(id)) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            //check if patient age is correct
            DateTime minDate = DateTime.Now.AddYears(-18),
                maxDate = DateTime.Now.AddYears(-5);
            if (model.BirthDate<minDate || model.BirthDate > maxDate)
            {
                ModelState.AddModelError("BirthDate", "Invalid Birth Date");
                return View(model);
            }

            Patient patient = new Patient()
            {
                BirthDate = model.BirthDate,
                FullName = model.FullName,
                UserId = id
            };
            patients.Add(patient);
            return RedirectToAction("Index");
        }
    }
}