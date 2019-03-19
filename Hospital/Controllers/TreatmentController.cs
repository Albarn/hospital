using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.Models;
using Hospital.Filters;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class TreatmentController : Controller
    {
        private IRepository<Treatment> treatments;
        private IRepository<Doctor> doctors;

        public TreatmentController(IRepository<Treatment> treatments, IRepository<Doctor> doctors)
        {
            this.treatments = treatments;
            this.doctors = doctors;
        }

        [Authorize(Roles = "Admin")] 
        public ActionResult New(string id)
        {
            throw new Exception();
            if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(CreateTreatmentViewModel model, string id)
        {
            if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            var doctor = doctors.Get(d=>d.FullName==model.DoctorFullName).SingleOrDefault();
            if (doctor == null)
            {
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
        
        public ActionResult Patient(string id)
        {
            if (User.IsInRole(Role.Patient.ToString())) id = UserService.GetUserId();
            else if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();

            ViewBag.Header = "Patient's Active Treatments";
            return View(SelectActiveTreatmentsItems(t => t.PatientId == id));
        }

        public ActionResult PatientArchive(string id)
        {
            if (User.IsInRole(Role.Patient.ToString())) id = UserService.GetUserId();
            else if (!UserService.IsUserInRole(id, Role.Patient)) return HttpNotFound();

            ViewBag.Header = "Patient's Finished Treatments";
            return View("Patient",SelectFinishedTreatmentsItems(t => t.PatientId == id));
        }

        

        [Authorize(Roles ="Admin, Doctor, Nurse")]
        public ActionResult Doctor(string id)
        {
            if (User.IsInRole(Role.Doctor.ToString()) && id == null) id = UserService.GetUserId();
            if (!UserService.IsUserInRole(id, Role.Doctor)) return HttpNotFound();

            ViewBag.Header = "Doctor's Active Treatments";
            return View(SelectActiveTreatmentsItems(t=>t.DoctorId==id));
        }

        [Authorize(Roles = "Admin, Doctor, Nurse")]
        public ActionResult DoctorArchive(string id)
        {
            if (User.IsInRole(Role.Doctor.ToString()) && id == null) id = UserService.GetUserId();
            if (!UserService.IsUserInRole(id, Role.Doctor)) return HttpNotFound();

            ViewBag.Header = "Doctor's Finished Treatments";
            return View("Doctor",SelectFinishedTreatmentsItems(t=>t.DoctorId==id));
        }

        public ActionResult Details(string id)
        {
            var treatment = treatments.Find(id);
            if (treatment == null) return HttpNotFound();

            if (User.IsInRole(Role.Patient.ToString()) &&
                treatment.PatientId != UserService.GetUserId())
            {
                return HttpNotFound();
            }
            return View(treatment);
        }

        [Authorize(Roles="Doctor")]
        public ActionResult Finish(string id)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public ActionResult Finish(string FinalDiagnosis, string id)
        {
            var treatment = treatments.Find(id);
            if (treatment == null) return HttpNotFound();

            if(FinalDiagnosis.Length<4 && FinalDiagnosis.Length > 30)
            {
                ModelState.AddModelError("", "Length of diagnosis must be between 4 and 30");
            }
            treatment.FinalDiagnosis = FinalDiagnosis;
            treatment.FinishDate = DateTime.Now;

            treatments.Update(treatment);
            return RedirectToAction("Doctor");
        }

        #region helpers

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

        IEnumerable<TreatmentItemViewModel> SelectActiveTreatmentsItems(Predicate<Treatment> condition)
        {
            return SelectTreatmentsItems(t => t.FinishDate == null && condition(t));
        }

        IEnumerable<TreatmentItemViewModel> SelectFinishedTreatmentsItems(Predicate<Treatment> condition)
        {
            return SelectTreatmentsItems(t => t.FinishDate != null && condition(t));
        }

        #endregion
    }
}