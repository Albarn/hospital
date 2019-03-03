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
            var userManager = new UserManager<User>(new UserStore());
            
            //create admin user with creds from config file
            userManager.Create(
                new User()
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUserName"],
                    RolesString = "admin",
                    IsConfirmed=true
                },
                WebConfigurationManager.AppSettings["AdminPassword"]);

            //create other users
            userManager.Create(
                new User()
                {
                    UserName = "john_d",
                    RolesString = "doctor",
                    IsConfirmed = true
                },
                "debug0");
            var res=userManager.Create(
                new User()
                {
                    UserName = "sasha_d",
                    RolesString = "doctor",
                    IsConfirmed = true
                },
                "debug0");

            //add related entities to users
            var u = context.Users.ToList();
            context.Doctors.Add(new Doctor()
            {
                FullName = "John Doe",
                Position = Position.Pediatrician,
                UserId = userManager.FindByName("john_d").Id
            });
            context.Doctors.Add(new Doctor()
            {
                FullName = "Sasha Kim",
                Position = Position.Surgeon,
                UserId = userManager.FindByName("sasha_d").Id
            });
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
