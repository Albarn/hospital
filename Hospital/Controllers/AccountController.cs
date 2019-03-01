using Hospital.DataAccess.Models;
using Hospital.Models;
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
    }
}