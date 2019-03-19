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
    [ExceptionHandlerAttribute]
    [RequestLogger]
    public class NurseController : Controller
    {
        private IRepository<Nurse> nurses;
        private Logger logger;

        public NurseController(IRepository<Nurse> nurses)
        {
            this.nurses = nurses;
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// gets list of nurses
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(nurses.GetAll().OrderBy(d => d.FullName));
        }

        public ActionResult Details(string id)
        {
            if (id == null && User.IsInRole(Role.Nurse.ToString()))
            {
                logger.Info("nurse goes to their page");
                id = UserService.GetUserId();
            }

            Nurse nurse = nurses.Find(id);
            if (nurse == null)
            {
                logger.Info("nurse not found, returning 404");
                return HttpNotFound();
            }
            else return View(nurse);
        }

        /// <summary>
        /// create nurse with given user
        /// </summary>
        [Authorize(Roles="Admin")]
        public ActionResult New(string id)
        {
            if (!UserService.IsUserInRole(id, Role.Nurse))
            {
                logger.Info("attempt to attach nurse to user with another role, returning 404");
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult New(CreateNurseViewModel model, string id)
        {
            if (!UserService.IsUserInRole(id, Role.Nurse))
            {
                logger.Info("attempt to attach nurse to user with another role, returning 404");
                return HttpNotFound();
            }
            if (!ModelState.IsValid)
            {
                logger.Info("model isn't valid, returning back");
                return View(model);
            }

            var nurse = new Nurse()
            {
                FullName = model.FullName,
                UserId = id
            };
            nurses.Add(nurse);
            UserService.SetUserConfirmed(id);
            return RedirectToAction("Index");
        }
    }
}