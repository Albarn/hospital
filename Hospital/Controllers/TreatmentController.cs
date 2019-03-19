using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.Models;
using Hospital.Filters;
using Hospital.Models;
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
    public class TreatmentController : Controller
    {
        private IRepository<Treatment> treatments;
        private IRepository<Doctor> doctors;
        private Logger logger;

        public TreatmentController(IRepository<Treatment> treatments, IRepository<Doctor> doctors)
        {
            this.treatments = treatments;
            this.doctors = doctors;
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// create new patient's treatment
        /// </summary>
        /// <param name="id">patient id</param>
        [Authorize(Roles = "Admin")] 
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("patient not found, returning 404");
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(CreateTreatmentViewModel model, string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("patient not found, returning 404");
                return HttpNotFound();
            }
            if (!ModelState.IsValid)
            {
                logger.Info("model is not valid, returning back");
                return View(model);
            }

            //find doctor who will lead patient's treatment
            var doctor = doctors.Get(d=>d.FullName==model.DoctorFullName).SingleOrDefault();
            if (doctor == null)
            {
                logger.Info("doctor not found, returning back");
                ModelState.AddModelError("DoctorFullName", "Doctor with this name doesn't exist.");
                return View(model);
            }
            Treatment treatment = new Treatment()
            {
                DoctorId = doctor.UserId,
                PatientId = id,
                Complaint = model.Complaint,
                StartDate = DateTime.Now
            };
            treatments.Add(treatment);
            return RedirectToAction("Details", "Patient", new { id = id });
        }
        
        /// <summary>
        /// gets patients treatments
        /// </summary>
        /// <param name="id">patient id</param>
        public ActionResult Patient(string id)
        {
            //patient can only see their treatments
            if (User.IsInRole(Role.Patient.ToString()))
            {
                logger.Info("patient goes to their treatments");
                id = UserService.GetUserId();
            }
            else if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("patient not found, returning 404");
                return HttpNotFound();
            }

            ViewBag.Header = "Patient's Active Treatments";
            return View(SelectActiveTreatmentsItems(t => t.PatientId == id));
        }

        /// <summary>
        /// gets patient's finished treatments
        /// </summary>
        public ActionResult PatientArchive(string id)
        {
            if (User.IsInRole(Role.Patient.ToString()))
            {
                logger.Info("patient goes to their treatments");
                id = UserService.GetUserId();
            }
            else if (!UserService.IsUserInRole(id, Role.Patient))
            {
                logger.Info("patient not found, returning 404");
                return HttpNotFound();
            }

            ViewBag.Header = "Patient's Finished Treatments";
            return View("Patient",SelectFinishedTreatmentsItems(t => t.PatientId == id));
        }

        /// <summary>
        /// gets doctor's active treatments
        /// </summary>
        [Authorize(Roles ="Admin, Doctor, Nurse")]
        public ActionResult Doctor(string id)
        {
            if (User.IsInRole(Role.Doctor.ToString()) && id == null)
            {
                logger.Info("doctor goes to their treatments");
                id = UserService.GetUserId();
            }
            if (!UserService.IsUserInRole(id, Role.Doctor))
            {
                logger.Info("doctor not found, returning 404");
                return HttpNotFound();
            }

            ViewBag.Header = "Doctor's Active Treatments";
            return View(SelectActiveTreatmentsItems(t=>t.DoctorId==id));
        }

        /// <summary>
        /// doctor's finished treatments
        /// </summary>
        [Authorize(Roles = "Admin, Doctor, Nurse")]
        public ActionResult DoctorArchive(string id)
        {
            if (User.IsInRole(Role.Doctor.ToString()) && id == null)
            {
                logger.Info("doctor goes to their treatments");
                id = UserService.GetUserId();
            }
            if (!UserService.IsUserInRole(id, Role.Doctor))
            {
                logger.Info("doctor not found, returning 404");
                return HttpNotFound();
            }

            ViewBag.Header = "Doctor's Finished Treatments";
            return View("Doctor",SelectFinishedTreatmentsItems(t=>t.DoctorId==id));
        }

        public ActionResult Details(string id)
        {
            var treatment = treatments.Find(id);
            if (treatment == null)
            {
                logger.Info("treatment not found, returning 404");
                return HttpNotFound();
            }

            //check availability for patient
            if (User.IsInRole(Role.Patient.ToString()) &&
                treatment.PatientId != UserService.GetUserId())
            {
                logger.Info("attempt to see private treatment, returning 404");
                return HttpNotFound();
            }

            //check user rights
            if (UserService.GetUserId() == treatment.DoctorId)
            {
                logger.Info("doctor has permission to modify treatment");
            }
            ViewBag.CanManage = true;
            return View(treatment);
        }

        /// <summary>
        /// make final diagnosis for patient
        /// </summary>
        [Authorize(Roles="Doctor")]
        public ActionResult Finish(string id)
        {
            var treatment = treatments.Find(id);
            if (treatment == null)
            {
                logger.Info("treatment not found, returning 404");
                return HttpNotFound();
            }
            if (treatment.DoctorId != UserService.GetUserId())
            {
                logger.Info("attempt to modify treatment without permission");
                return View("Error");
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public ActionResult Finish(string FinalDiagnosis, string id)
        {
            var treatment = treatments.Find(id);
            if (treatment == null)
            {
                logger.Info("treatment not found, returning 404");
                return HttpNotFound();
            }
            if (treatment.DoctorId != UserService.GetUserId())
            {
                logger.Info("attempt to modify treatment without permission");
                return View("Error");
            }

            if (FinalDiagnosis.Length<4 && FinalDiagnosis.Length > 30)
            {
                logger.Info("illegal diagnosis given, returning back");
                ModelState.AddModelError("", "Length of diagnosis must be between 4 and 30");
            }
            treatment.FinalDiagnosis = FinalDiagnosis;
            treatment.FinishDate = DateTime.Now;

            treatments.Update(treatment);
            return RedirectToAction("Doctor");
        }

        #region helpers

        /// <summary>
        /// selects list of TreatmentItemViewModel from repo
        /// </summary>
        IEnumerable<TreatmentItemViewModel> SelectTreatmentsItems(Predicate<Treatment> condition)
        {
            return treatments
                .Get(t => condition(t))
                .Select(t => new TreatmentItemViewModel()
                {
                    Diagnosis = t.Complaint,
                    PatientId = t.PatientId,
                    DoctorId = t.DoctorId,
                    StartDate = t.StartDate,
                    TreatmentId = t.TreatmentId
                });
        }

        /// <summary>
        /// selects items with null finish date
        /// </summary>
        IEnumerable<TreatmentItemViewModel> SelectActiveTreatmentsItems(Predicate<Treatment> condition)
        {
            return SelectTreatmentsItems(t => t.FinishDate == null && condition(t));
        }

        /// <summary>
        /// selects items with existing finish date
        /// </summary>
        IEnumerable<TreatmentItemViewModel> SelectFinishedTreatmentsItems(Predicate<Treatment> condition)
        {
            return SelectTreatmentsItems(t => t.FinishDate != null && condition(t));
        }

        #endregion
    }
}