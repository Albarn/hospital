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
    public class AccountController : Controller
    {
        private SignInManager<User, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<User, string>>();
        private Logger logger;

        public AccountController()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                logger.Info("model state not valid, returning back");
                return View(model);
            }

            //try to sign in with given password
            var loginResult = SignInManager.PasswordSignIn(model.UserName, model.Password, true, false);
            if (loginResult != SignInStatus.Success)
            {
                logger.Info("invalid login attempt");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            
            //check IsConfirmed property
            var user = SignInManager.UserManager.FindByName(model.UserName);
            if (user != null && user.IsConfirmed != true)
            {
                logger.Info("user isn't confirmed, sign out");
                SignInManager.AuthenticationManager.SignOut();
                ModelState.AddModelError("", "Registration is not completed.");
                return View(model);
            }

            logger.Info("login succeded");
            if (returnUrl != null)
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            logger.Info("sigining out");
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// register user with role
        /// </summary>
        [Authorize(Roles = "Admin")]
        public ActionResult Register(Role role)
        {
            if (role == Role.Admin)
            {
                logger.Info("attempt to register admin, returning 404");
                return HttpNotFound();
            }
            ViewBag.Role = role.ToString();
            return View();
        }

        /// <summary>
        /// create user and redirect Admin to next registration form
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Register(RegisterViewModel model, Role role)
        {
            //there is must be one admin
            if (role == Role.Admin)
            {
                logger.Info("attempt to register admin, returning 404");
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                logger.Info("model state not valid, returning back");
                return View(model);
            }

            //check name availability
            if (SignInManager.UserManager.FindByName(model.UserName) != null)
            {
                logger.Info("name is not available, returning back");
                ModelState.AddModelError("", "This User Name is not available.");
                return View(model);
            }

            //create user
            var user = new User()
            {
                UserName = model.UserName,
                IsConfirmed = false,
                Roles = (int)role
            };
            var res = SignInManager.UserManager.Create(user, model.Password);
            if (!res.Succeeded)
            {
                logger.Error("Failed to create user" + res.Errors.Aggregate((s1, s2) => s1 + "\n" + s2));
                ModelState.AddModelError("", "Failed to create user, try again.");
                return View(model);
            }

            logger.Info("user with role created, continue registration");
            return RedirectToAction("New", role.ToString(), new { id = user.Id });
        }
    }
}