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

            //check IsConfirmed property
            var user = SignInManager.UserManager.FindByName(model.UserName);
            if (user != null && user.IsConfirmed != true)
            {
                ModelState.AddModelError("", "Registration is not completed.");
                return View(model);
            }

            var loginResult = SignInManager.PasswordSignIn(model.UserName, model.Password, true, false);
            if (loginResult != SignInStatus.Success)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Register(Role role)
        {
            if (role == Role.Admin) return HttpNotFound();
            ViewBag.Role = role.ToString();
            return View();
        }

        //create user and redirect Admin to next registration form
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Register(RegisterViewModel model, Role role)
        {
            if (role == Role.Admin) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);
            if (SignInManager.UserManager.FindByName(model.UserName) != null)
            {
                ModelState.AddModelError("", "This User Name is not available.");
                return View(model);
            }
            var user = new User()
            {
                UserName = model.UserName,
                IsConfirmed = false,
                Roles = (int)role
            };
            var res = SignInManager.UserManager.Create(user, model.Password);
            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Failed to create user, try again.");
                return View(model);
            }
            return RedirectToAction("New", role.ToString(), new { id = user.Id });
        }
    }
}