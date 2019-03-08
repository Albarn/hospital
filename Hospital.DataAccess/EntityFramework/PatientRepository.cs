using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class PatientRepository : IRepository<Patient>
    {
        private HospitalDbContext db = HospitalDbContext.Create();
        public void Add(Patient entity)
        {
            db.Patients.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Patient entity)
        {
            db.Patients.Remove(entity);
            db.SaveChanges();
        }

        public Patient Find(params object[] keys)
        {
            return db.Patients.Find(keys);
        }

        public IEnumerable<Patient> Get(Func<Patient, bool> condition)
        {
            return db.Patients.Where(condition).ToList();
        }

        public IEnumerable<Patient> GetAll()
        {
            return db.Patients.ToList();
        }

        public void Update(Patient entity)
        {
            var entry = db.Patients.Find(entity.UserId);
            db.Entry(entry).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }
}
