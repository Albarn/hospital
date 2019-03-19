using Hospital.DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.BLL
{
    /// <summary>
    /// helpers to work with users
    /// </summary>
    public static class UserService
    {
        private static Logger Logger { get => LogManager.GetCurrentClassLogger(); }

        /// <summary>
        /// checks if user with id in role,
        /// returns false if user doesn't exist
        /// </summary>
        public static bool IsUserInRole(string id, Role role)
        {
            Logger.Info("checking user role");
            return HttpContext
                .Current
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindById(id)
                ?.IsInRole(role) ?? false;
        }

        /// <summary>
        /// get current user id
        /// </summary>
        public static string GetUserId()
        {
            Logger.Info("getting current user id");
            return HttpContext
                .Current
                .GetOwinContext()
                .Get<UserManager<User>>()
                .FindByName(HttpContext.Current.User.Identity.Name)
                ?.Id;
        }

        /// <summary>
        /// sets IsConfirmed property
        /// </summary>
        /// <param name="id"></param>
        public static void SetUserConfirmed(string id)
        {
            Logger.Info("setting user confirmed");
            var manager = HttpContext
                .Current
                .GetOwinContext()
                .Get<UserManager<User>>();

            var user = manager.FindById(id);
            user.IsConfirmed = true;
            manager.Update(user);
        }
    }
}