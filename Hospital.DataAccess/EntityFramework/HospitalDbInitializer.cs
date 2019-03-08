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

            //create Admin user with creds from config file
            userManager.Create(
                new User()
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUserName"],
                    Roles = (int)Role.Admin,
                    IsConfirmed = true
                },
                WebConfigurationManager.AppSettings["AdminPassword"]);

            //create other users
            User
                d1 = new User() { UserName = "john_d", Roles = (int)Role.Doctor, IsConfirmed = true },
                d2 = new User() { UserName = "sasha_d", Roles = (int)Role.Doctor, IsConfirmed = true },
                n1 = new User() { UserName = "mike_n", Roles = (int)Role.Nurse, IsConfirmed = true },
                n2 = new User() { UserName = "kate_n", Roles = (int)Role.Nurse, IsConfirmed = true },
                p1 = new User() { UserName = "pike_p", Roles = (int)Role.Patient, IsConfirmed = true },
                p2 = new User() { UserName = "poppy_p", Roles = (int)Role.Patient, IsConfirmed = true };

            string pass = "debug0";
            userManager.Create(d1, pass);
            userManager.Create(d2, pass);
            userManager.Create(n1, pass);
            userManager.Create(n2, pass);
            userManager.Create(p1, pass);
            userManager.Create(p2, pass);

            //add related entities to users
            context.Doctors.Add(
                new Doctor() { FullName = "John Doe", Position = Position.Pediatrician, UserId = d1.Id });
            context.Doctors.Add(
                new Doctor() { FullName = "Sasha Kim", Position = Position.Surgeon, UserId = d2.Id });
            context.Nurses.Add(new Nurse() { FullName = "Mike Silly", UserId = n1.Id });
            context.Nurses.Add(new Nurse() { FullName = "Kate Brave", UserId = n2.Id });
            context.Patients.Add(new Patient() { FullName = "Pike Liar", UserId = p1.Id, BirthDate = new DateTime(2012, 1, 10) });
            context.Patients.Add(new Patient() { FullName = "Poppy Graver", UserId = p2.Id, BirthDate = new DateTime(2010, 10, 1) });

            context.SaveChanges();
            base.Seed(context);
        }
    }
}
