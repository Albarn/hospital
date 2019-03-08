using Hospital.DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.BLL
{
    public static class UserService
    {
        public static bool IsUserInRole(string id, Role role)
        {
            return HttpContext
                .Current
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindById(id)
                ?.IsInRole(role) ?? false;
        }
    }
}