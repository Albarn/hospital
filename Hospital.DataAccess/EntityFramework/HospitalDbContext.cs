using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    class HospitalDbContext:DbContext
    {
        public HospitalDbContext() : base("Default") { }
        static HospitalDbContext() { Database.SetInitializer(new HospitalDbInitializer()); }

        public IDbSet<User> Users { get; set; }
        public IDbSet<Doctor> Doctors { get; set; }
        public IDbSet<Nurse> Nurses { get; set; }

        private static HospitalDbContext instance;
        public static HospitalDbContext Create()
        {
            if (instance == null) instance = new HospitalDbContext();
            return instance;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Doctor)
                .WithRequired(d => d.User);

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Nurse)
                .WithRequired(n => n.User);

            base.OnModelCreating(modelBuilder);
        }
    }
}
