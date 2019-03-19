using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.Models;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private IRepository<Assignment> assignments;
        private IRepository<Treatment> treatments;

        public AssignmentController(IRepository<Assignment> assignments, IRepository<Treatment> treatments)
        {
            this.assignments = assignments;
            this.treatments = treatments;
        }

        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult Index()
        {
            string id = UserService.GetUserId();
            return View(assignments
                .Get(a => a.FinishDate == null && (a.DoctorId == id || a.NurseId == id))
                .OrderBy(a=>a.AssignmentDate));
        }

        [Authorize(Roles="Doctor")]
        public ActionResult New(string id)
        {
            if (treatments.Find(id)?.DoctorId == UserService.GetUserId())
                return View();
            else
                return HttpNotFound();
        }

        [HttpPost]
        [Authorize(Roles="Doctor")]
        public ActionResult New(CreateAssignmentViewModel model, string id)
        {
            if (!(treatments.Find(id)?.DoctorId == UserService.GetUserId()))
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.AssignmentDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("AssignmentDate", "Assignment date cannot be in the past.");
                return View(model);
            }

            Assignment assignment = new Assignment()
            {
                TreatmentId = id,
                AssignmentDate = model.AssignmentDate,
                Diagnosis = model.Diagnosis,
                Instruction = model.Instruction,
                Type = model.Type
            };

            if (model.IsDoctor)
            {
                var assigned = UserService.GetUserByName(model.AssignedFullName);
                if (assigned==null || !assigned.IsInRole(Role.Doctor))
                {
                    ModelState.AddModelError("AssignedFullName", "Doctor with this name doesn't exist.");
                    return View(model);
                }
                assignment.DoctorId = assigned.Id;
            }
            else
            {
                if (model.Type == AssignmentType.Operation)
                {
                    ModelState.AddModelError("Type", "Nurse cannot be assigned to operation.");
                    return View(model);
                }
                var assigned = UserService.GetUserByName(model.AssignedFullName);
                if (assigned == null || !assigned.IsInRole(Role.Doctor))
                {
                    ModelState.AddModelError("AssignedFullName", "Nurse with this name doesn't exist.");
                    return View(model);
                }
                assignment.NurseId = assigned.Id;
            }

            assignments.Add(assignment);
            return RedirectToAction("Details", "Treatment", new { id = id });
        }

        

        [HttpPost]
        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult Finish(string id)
        {
            var assignment = assignments.Find(id);
            if (assignment == null) return HttpNotFound();

            string userId = UserService.GetUserId();
            if (userId!=assignment.DoctorId && userId!=assignment.NurseId)
            {
                return HttpNotFound();
            }

            if (assignment.AssignmentDate.Date > DateTime.Now.Date)
            {
                return new HttpStatusCodeResult(500);
            }
            assignment.FinishDate = DateTime.Now;
            assignments.Update(assignment);
            return PartialView("EmptyPartial");
        }
    }
}