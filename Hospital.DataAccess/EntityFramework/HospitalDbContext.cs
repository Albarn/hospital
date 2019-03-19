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
        public IDbSet<Patient> Patients { get; set; }
        public IDbSet<Treatment> Treatments { get; set; }
        public IDbSet<Assignment> Assignments { get; set; }

        //for single instance support
        private static HospitalDbContext instance;
        public static HospitalDbContext Create()
        {
            if (instance == null) instance = new HospitalDbContext();
            return instance;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //user can be doctor or nurse or patient
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Doctor)
                .WithRequired(d => d.User);

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Nurse)
                .WithRequired(n => n.User);

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Patient)
                .WithRequired(p => p.User);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Treatments)
                .WithRequired(t => t.Doctor)
                .HasForeignKey(t => t.DoctorId);

            //patient has many treatments
            modelBuilder.Entity<Patient>()
                .HasMany(d => d.Treatments)
                .WithRequired(t => t.Patient)
                .HasForeignKey(t => t.PatientId);

            //treatments have many assignments
            modelBuilder.Entity<Treatment>()
                .HasMany(t => t.Assignments)
                .WithRequired(t => t.Treatment)
                .HasForeignKey(t => t.TreatmentId);

            //doctor or nurse can be assigned
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Assignments)
                .WithOptional(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Nurse>()
                .HasMany(n => n.Assignments)
                .WithOptional(a => a.Nurse)
                .HasForeignKey(a => a.NurseId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
