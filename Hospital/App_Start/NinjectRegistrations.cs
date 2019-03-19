using Hospital.DataAccess;
using Hospital.DataAccess.EntityFramework;
using Hospital.DataAccess.Models;
using Microsoft.AspNet.Identity;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace Hospital
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserStore<User>>().To<UserStore>();
            Bind<IRepository<Doctor>>().To<DoctorRepository>();
            Bind<IRepository<Nurse>>().To<NurseRepository>();
            Bind<IRepository<Patient>>().To<PatientRepository>();
            Bind<IRepository<Treatment>>().To<TreatmentRepository>();
            Bind<IRepository<Assignment>>().To<AssignmentRepository>();
        }
    }
}