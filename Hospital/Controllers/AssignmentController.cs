using Hospital.BLL;
using Hospital.DataAccess;
using Hospital.DataAccess.EntityFramework;
using Hospital.DataAccess.Models;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    public class AssignmentController : Controller
    {
        private IRepository<Assignment> assignments = new AssignmentRepository();
        
        public ActionResult New(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(CreateAssignmentViewModel model, string id)
        {
            if (!ModelState.IsValid)
            {
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
                var doctors = new DoctorRepository();
                var assigned = doctors.Get(d=>d.FullName==model.AssignedFullName).SingleOrDefault();
                assignment.DoctorId = assigned.UserId;
            }
            else
            {
                var nurses = new NurseRepository();
                var assigned = nurses.Get(d => d.FullName == model.AssignedFullName).SingleOrDefault();
                assignment.DoctorId = assigned.UserId;
            }

            assignments.Add(assignment);
            return RedirectToAction("Details", "Treatment", new { id = id });
        }
    }
}