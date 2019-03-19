using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.Models;
using Hospital.Filters;
using Hospital.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    [Authorize]
    [ExceptionHandler]
    [RequestLogger]
    public class PatientController : Controller
    {
        private IRepository<Patient> patients;
        private Logger logger;

        public PatientController(IRepository<Patient> patients)
        {
            this.patients = patients;
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// get list of patients
        /// </summary>
        [Authorize(Roles = "Admin, Doctor, Nurse")]
        public ActionResult Index()
        {
            return View(patients.GetAll());
        }

        /// <summary>
        /// create patient with given user
        /// </summary>
        [Authorize(Roles="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("attempt to attach patient to user with another role, returning 404");
                return HttpNotFound();
            }

            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult New(CreatePatientViewModel model,string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("attempt to attach patient to user with another role, returning 404");
                return HttpNotFound();
            }
            if (!ModelState.IsValid)
            {
                logger.Info("model is not valid, returning back");
                return View(model);
            }

            //check if patient age is correct
            DateTime minDate = DateTime.Now.AddYears(-18),
                maxDate = DateTime.Now.AddYears(-5);
            if (model.BirthDate<minDate || model.BirthDate > maxDate)
            {
                logger.Info("user gave illegal patient age, returning back");
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
            //patient is only able to see their page, not others
            if (User.IsInRole("Patient"))
            {
                logger.Info("patient goes to their page");
                id = UserService.GetUserId();
            }

            var patient = patients.Find(id);
            if (patient == null)
            {
                logger.Info("patient not found, returning 404");
                return HttpNotFound();
            }
            return View(patient);
        }
    }
}