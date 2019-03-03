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
    [Authorize]
    public class AccountController : Controller
    {
        private SignInManager<User, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<User, string>>();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (SignInManager.UserManager.FindByName(model.UserName)?.IsConfirmed != true)
            {
                ModelState.AddModelError("", "Registration is not completed");
                return View(model);
            }
            var loginResult=SignInManager.PasswordSignIn(model.UserName, model.Password, true, false);
            if (loginResult != SignInStatus.Success)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //create user and redirect admin to next registration form
        private ActionResult RegisterWithRole(RegisterViewModel model, string role, string controllerNameToRedirect)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new User()
            {
                UserName = model.UserName,
                IsConfirmed = false,
                RolesString = role
            };
            var res = SignInManager.UserManager.Create(user, model.Password);
            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Failed to create user, try again.");
                return View(model);
            }
            return RedirectToAction("New", controllerNameToRedirect, new { id = user.Id });
        }

        [Authorize(Roles ="admin")]
        public ActionResult NewDoctor()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult NewDoctor(RegisterViewModel model)
        {
            return RegisterWithRole(model, "doctor", "Doctor");
        }
    }
}