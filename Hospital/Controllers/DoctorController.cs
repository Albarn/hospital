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
    [ExceptionHandler]
    [RequestLogger]
    public class DoctorController : Controller
    {
        private IRepository<Doctor> doctors;
        private IRepository<Treatment> treatments;
        private Logger logger;

        public DoctorController(IRepository<Doctor> doctors, IRepository<Treatment> treatments)
        {
            this.doctors = doctors;
            this.treatments = treatments;
            logger = LogManager.GetCurrentClassLogger();
        }
        
        /// <summary>
        /// gets doctors list with their patients number
        /// </summary>
        public ActionResult Index()
        {
            return View(PatientsCounter
                .CalculatePatientsNumber(doctors.GetAll(),treatments.GetAll())
                .OrderBy(d=>d.FullName));
        }

        /// <summary>
        /// gets doctors by categories
        /// </summary>
        /// <returns></returns>
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
                //if user is doctor, get their id
                if (User.IsInRole(Role.Doctor.ToString()))
                {
                    logger.Info("doctor goes to their profile page");
                    id = UserService.GetUserId();
                }
                else
                {
                    logger.Info("attempt to get doctor without id, returning 404");
                    return HttpNotFound();
                }
            }
            Doctor doctor = doctors.Find(id);
            if (doctor != null)
            {
                return View(doctors.Find(id));
            }
            else
            {
                logger.Info("doctor not found, returning 404");
                return HttpNotFound();
            }
        }

        /// <summary>
        /// create doctor with given user
        /// </summary>
        [Authorize(Roles ="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Doctor))
            {
                logger.Info("attempt to attach doctor to user with another role, returning 404");
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(string id, CreateDoctorViewModel model)
        {
            if (!UserService.IsUserInRole(id, Role.Doctor))
            {
                logger.Info("attempt to attach doctor to user with another role, returning 404");
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                logger.Info("model is not valid, returning back");
                return View(model);
            }

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