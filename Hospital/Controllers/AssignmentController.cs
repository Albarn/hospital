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
    public class AssignmentController : Controller
    {
        private IRepository<Assignment> assignments;
        private IRepository<Treatment> treatments;
        private IRepository<Doctor> doctors;
        private IRepository<Nurse> nurses;
        private Logger logger;

        public AssignmentController(
            IRepository<Assignment> assignments, 
            IRepository<Treatment> treatments, 
            IRepository<Doctor> doctors, 
            IRepository<Nurse> nurses)
        {
            this.assignments = assignments;
            this.treatments = treatments;
            this.doctors = doctors;
            this.nurses = nurses;
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// gets list of nurse's or doctor's assignments
        /// </summary>
        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult Index()
        {
            //get their id
            string id = UserService.GetUserId();

            //assignment active, so finish date is null
            return View(assignments
                .Get(a => a.FinishDate == null && (a.DoctorId == id || a.NurseId == id))
                .OrderBy(a=>a.AssignmentDate));
        }

        /// <summary>
        /// gets list of finished nurse's or doctor's assignments
        /// </summary>
        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult Archive()
        {
            string id = UserService.GetUserId();
            //assignment finished, so finish date is not null
            return View(assignments
                .Get(a => a.FinishDate != null && (a.DoctorId == id || a.NurseId == id))
                .OrderBy(a => a.AssignmentDate));
        }

        public ActionResult Details(string id)
        {
            Assignment assignment = assignments.Find(id);
            if (assignment == null)
            {
                logger.Info("assignment not found, returning 404");
                return HttpNotFound();
            }

            //if request from patient, check that it 
            //belongs to them
            if (User.IsInRole(Role.Patient.ToString()) &&
                assignment.Treatment.PatientId != UserService.GetUserId())
            {
                logger.Info("attempt to view private assignment, returning 404");
                return HttpNotFound();
            }

            return View(assignment);
        }

        /// <summary>
        /// gets assignments that doctor or nurse assigned to
        /// </summary>
        [Authorize(Roles="Admin, Doctor, Nurse")]
        public ActionResult Assigned(string id)
        {
            return View("List",assignments
                .Get(a => a.FinishDate == null && (a.DoctorId == id || a.NurseId == id))
                .OrderBy(a => a.AssignmentDate));
        }

        /// <summary>
        /// gets patient's assignments
        /// </summary>
        public ActionResult Patient(string id)
        {
            //if user is patient, user can see only their assignments,
            //so id equal to their id
            if (User.IsInRole(Role.Patient.ToString())) id = UserService.GetUserId();

            return View("List",assignments
                .Get(a => a.FinishDate == null && (a.Treatment.PatientId==id))
                .OrderBy(a => a.AssignmentDate));
        }

        /// <summary>
        /// gets assigned finished assignments
        /// </summary>
        [Authorize(Roles = "Admin, Doctor, Nurse")]
        public ActionResult AssignedArchive(string id)
        {
            return View("Archive", assignments
                .Get(a => a.FinishDate != null && (a.DoctorId == id || a.NurseId == id))
                .OrderBy(a => a.AssignmentDate));
        }

        /// <summary>
        /// gets patients finished assignments
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PatientArchive(string id)
        {
            if (User.IsInRole(Role.Patient.ToString())) id = UserService.GetUserId();
            return View("Archive", assignments
                .Get(a => a.FinishDate != null && (a.Treatment.PatientId == id))
                .OrderBy(a => a.AssignmentDate));
        }

        [Authorize(Roles="Doctor")]
        public ActionResult New(string id)
        {
            //doctor can make assignments only for their treatments
            if (treatments.Find(id)?.DoctorId == UserService.GetUserId())
            {
                return View();
            }
            else
            {
                logger.Info("attempt to manage private treatment, returning 404");
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles="Doctor")]
        public ActionResult New(CreateAssignmentViewModel model, string id)
        {
            //doctor can make assignments only for their treatments
            if (!(treatments.Find(id)?.DoctorId == UserService.GetUserId()))
            {
                logger.Info("attempt to manage private treatment, returning 404");
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                logger.Info("model isn't valid, returning back");
                return View(model);
            }

            //check assignment date
            if (model.AssignmentDate.Date < DateTime.Now.Date)
            {
                logger.Info("attempt to create past assignment, returning back");
                ModelState.AddModelError("AssignmentDate", "Assignment date cannot be in the past.");
                return View(model);
            }

            //creating assignment
            Assignment assignment = new Assignment()
            {
                TreatmentId = id,
                AssignmentDate = model.AssignmentDate,
                Diagnosis = model.Diagnosis,
                Instruction = model.Instruction,
                Type = model.Type
            };

            //make doctor or nurse assigned, depending on users choise
            if (model.IsDoctor)
            {
                var assigned = doctors.Get(d=>d.FullName==model.AssignedFullName).SingleOrDefault();
                if (assigned==null)
                {
                    logger.Info("user gave not real doctor name, returning back");
                    ModelState.AddModelError("AssignedFullName", "Doctor with this name doesn't exist.");
                    return View(model);
                }
                assignment.DoctorId = assigned.UserId;
            }
            else
            {
                //check type availability
                if (model.Type == AssignmentType.Operation)
                {
                    logger.Info("attempt to assign nurse to operation");
                    ModelState.AddModelError("Type", "Nurse cannot be assigned to operation.");
                    return View(model);
                }
                var assigned = nurses.Get(n => n.FullName == model.AssignedFullName).SingleOrDefault();
                if (assigned == null)
                {
                    logger.Info("user gave not real nurse name, returning back");
                    ModelState.AddModelError("AssignedFullName", "Nurse with this name doesn't exist.");
                    return View(model);
                }
                assignment.NurseId = assigned.UserId;
            }

            assignments.Add(assignment);
            logger.Info("assignment created");
            return RedirectToAction("Details", "Treatment", new { id = id });
        }

        
        /// <summary>
        /// mark assignment as done
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult Finish(string id)
        {
            var assignment = assignments.Find(id);
            if (assignment == null)
            {
                logger.Info("assignment not found, returning 404");
                return HttpNotFound();
            }

            string userId = UserService.GetUserId();
            if (userId!=assignment.DoctorId && userId!=assignment.NurseId)
            {
                logger.Info("user not assigned, returning 404");
                return HttpNotFound();
            }

            if (assignment.AssignmentDate.Date > DateTime.Now.Date)
            {
                logger.Error("attempt to finish assignment in the future");
                return new HttpStatusCodeResult(500);
            }
            assignment.FinishDate = DateTime.Now;
            assignments.Update(assignment);
            logger.Info("assignment finished");
            return PartialView("EmptyPartial");
        }
    }
}