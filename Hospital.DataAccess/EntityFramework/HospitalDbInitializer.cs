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
            var log = NLog.LogManager.GetCurrentClassLogger();
            log.Info("in db seed method");

            var userManager = new UserManager<User>(new UserStore());

            log.Info("register admin user");
            //create Admin user with creds from config file
            var identityResult = userManager.Create(
                new User()
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUserName"],
                    Roles = (int)Role.Admin,
                    IsConfirmed = true
                },
                WebConfigurationManager.AppSettings["AdminPassword"]);
            if (identityResult.Succeeded)
            {
                log.Debug("register admin succeded");
            }
            else
            {
                log.Debug("failed to register admin: " + identityResult.Errors.Last());
            }

            //create other users
            User[] doctors = new[]
            {
                new User() { UserName = "john_d",  Roles = (int)Role.Doctor, IsConfirmed = true },
                new User() { UserName = "sasha_d", Roles = (int)Role.Doctor, IsConfirmed = true },
                new User() { UserName = "mika_d",  Roles = (int)Role.Doctor, IsConfirmed = true },
                new User() { UserName = "tom_d",   Roles = (int)Role.Doctor, IsConfirmed = true },
                new User() { UserName = "jack_d",  Roles = (int)Role.Doctor, IsConfirmed = true },
            };
            User[] nurses = new[] {
                new User() { UserName = "mike_n",   Roles = (int)Role.Nurse, IsConfirmed = true },
                new User() { UserName = "kate_n",   Roles = (int)Role.Nurse, IsConfirmed = true },
                new User() { UserName = "hommy_n",  Roles = (int)Role.Nurse, IsConfirmed = true },
                new User() { UserName = "stasey_n", Roles = (int)Role.Nurse, IsConfirmed = true },
                new User() { UserName = "rick_n",   Roles = (int)Role.Nurse, IsConfirmed = true },
            };
            User[] patients = new[] {
                new User() { UserName = "pike_p",  Roles = (int)Role.Patient, IsConfirmed = true },
                new User() { UserName = "poppy_p", Roles = (int)Role.Patient, IsConfirmed = true },
                new User() { UserName = "ira_p",   Roles = (int)Role.Patient, IsConfirmed = true },
                new User() { UserName = "stan_p",  Roles = (int)Role.Patient, IsConfirmed = true },
                new User() { UserName = "pobby_p", Roles = (int)Role.Patient, IsConfirmed = true }
            };

            string pass = "debug0";
            foreach (var u in doctors.Concat(nurses).Concat(patients))
            {
                identityResult = userManager.Create(u, pass);
                if (identityResult.Succeeded)
                {
                    log.Debug("user " + u.UserName + " registred");
                }
                else
                {
                    log.Debug("failed to register user " + u.UserName + ":" + identityResult.Errors.Last());
                }
            }

            log.Info("creating doctors");
            //add related entities to users
            context.Doctors.Add(new Doctor() { FullName = "John Doe", Position = Position.Pediatrician, UserId = doctors[0].Id });
            context.Doctors.Add(new Doctor() { FullName = "Sasha Kim", Position = Position.Surgeon, UserId = doctors[1].Id });
            context.Doctors.Add(new Doctor() { FullName = "Mika Kim", Position = Position.Surgeon, UserId = doctors[2].Id });
            context.Doctors.Add(new Doctor() { FullName = "Tom Sunny", Position = Position.Traumatologist, UserId = doctors[3].Id });
            context.Doctors.Add(new Doctor() { FullName = "Jack Jack", Position = Position.Surgeon, UserId = doctors[4].Id });

            log.Info("creating nurses");
            context.Nurses.Add(new Nurse() { FullName = "Mike Silly", UserId = nurses[0].Id });
            context.Nurses.Add(new Nurse() { FullName = "Kate Brave", UserId = nurses[1].Id });
            context.Nurses.Add(new Nurse() { FullName = "Hommy Dead", UserId = nurses[2].Id });
            context.Nurses.Add(new Nurse() { FullName = "Stasey Spy", UserId = nurses[3].Id });
            context.Nurses.Add(new Nurse() { FullName = "Rick Brave", UserId = nurses[4].Id });
            
            log.Info("creating patients");
            context.Patients.Add(new Patient() { FullName = "Pike Liar", BirthDate = new DateTime(2012, 1, 10), UserId = patients[0].Id });
            context.Patients.Add(new Patient() { FullName = "Poppy Graver", BirthDate = new DateTime(2010, 10, 1), UserId = patients[1].Id });
            context.Patients.Add(new Patient() { FullName = "Ira Graver", BirthDate = new DateTime(2011, 08, 15), UserId = patients[2].Id });
            context.Patients.Add(new Patient() { FullName = "Stan Graver", BirthDate = new DateTime(2004, 6, 2), UserId = patients[3].Id });
            context.Patients.Add(new Patient() { FullName = "Pobby Coward", BirthDate = new DateTime(2013, 3, 15), UserId = patients[4].Id });

            log.Info("creating treatments");
            Treatment[] treatments = new[]
            {
                new Treatment(){ PatientId = patients[0].Id, DoctorId = doctors[0].Id, StartDate = new DateTime(2019,03,14), Complaint = "headache" },
                new Treatment(){ PatientId = patients[0].Id, DoctorId = doctors[1].Id, StartDate = new DateTime(2019,03,11), Complaint = "vomiting" },
                new Treatment(){ PatientId = patients[0].Id, DoctorId = doctors[4].Id, StartDate = new DateTime(2019,02,14), Complaint = "anxiousness" },
                new Treatment(){ PatientId = patients[1].Id, DoctorId = doctors[0].Id, StartDate = new DateTime(2019,03,08), Complaint = "bad stomach" },
                new Treatment(){ PatientId = patients[2].Id, DoctorId = doctors[0].Id, StartDate = new DateTime(2019,03,13), Complaint = "headache" }
            };

            log.Info("creating assignments");
            treatments[0].Assignments.Add(new Assignment()
            {
                TreatmentId = treatments[0].TreatmentId,
                Diagnosis = "cold",
                DoctorId = doctors[1].Id,
                AssignmentDate = new DateTime(2019, 03, 15),
                Type = AssignmentType.Medicine,
                Instruction = "give him some pills"
            });
            treatments[1].Assignments.Add(new Assignment()
            {
                TreatmentId = treatments[0].TreatmentId,
                Diagnosis = "poisoning",
                DoctorId = doctors[1].Id,
                AssignmentDate = new DateTime(2019, 03, 16),
                Type = AssignmentType.Medicine,
                Instruction = "give him some pills"
            });
            treatments[2].Assignments.Add(new Assignment()
            {
                TreatmentId = treatments[0].TreatmentId,
                Diagnosis = "overworked",
                DoctorId = doctors[1].Id,
                AssignmentDate = new DateTime(2019, 03, 17),
                Type = AssignmentType.Procedure,
                Instruction = "watch cartoons"
            });
            treatments[3].Assignments.Add(new Assignment()
            {
                TreatmentId = treatments[0].TreatmentId,
                Diagnosis = "worm",
                DoctorId = doctors[2].Id,
                AssignmentDate = new DateTime(2019, 03, 17),
                Type = AssignmentType.Operation,
                Instruction = "extract worm from stomach"
            });
            treatments[4].Assignments.Add(new Assignment()
            {
                TreatmentId = treatments[0].TreatmentId,
                Diagnosis = "cold",
                NurseId = nurses[0].Id,
                AssignmentDate = new DateTime(2019, 03, 17),
                Type = AssignmentType.Medicine,
                Instruction = "give him some pills"
            });

            foreach (var t in treatments)
                context.Treatments.Add(t);

            log.Info("saving changes to the context");
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
