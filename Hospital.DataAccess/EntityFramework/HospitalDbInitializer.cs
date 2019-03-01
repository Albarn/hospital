using Hospital.DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Hospital.DataAccess.EntityFramework
{
    class HospitalDbInitializer : DropCreateDatabaseIfModelChanges<HospitalDbContext>
    {
        protected override void Seed(HospitalDbContext context)
        {
            var userManager = HttpContext
                .Current
                .GetOwinContext()
                .Get<UserManager<User>>();
            
            //create admin user with creds from config file
            userManager.Create(
                new User()
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUserName"],
                    RolesString = "admin"
                },
                WebConfigurationManager.AppSettings["AdminPassword"]);
            base.Seed(context);
        }
    }
}
