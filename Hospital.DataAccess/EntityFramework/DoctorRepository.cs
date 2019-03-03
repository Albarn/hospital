using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private HospitalDbContext db = HospitalDbContext.Create();
        public void Add(Doctor entity)
        {
            db.Doctors.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Doctor entity)
        {
            db.Doctors.Remove(entity);
            db.SaveChanges();
        }

        public Doctor Find(params object[] keys)
        {
            return db.Doctors.Find(keys);
        }

        public IEnumerable<Doctor> Get(Func<Doctor, bool> condition)
        {
            return db.Doctors.Where(condition).ToList();
        }

        public IEnumerable<Doctor> GetAll()
        {
            return db.Doctors.ToList();
        }

        public void Update(Doctor entity)
        {
            var entry = db.Doctors.Find(entity.UserId);
            db.Entry(entry).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }
}
