using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class TreatmentRepository : IRepository<Treatment>
    {
        private HospitalDbContext db = HospitalDbContext.Create();
        public void Add(Treatment entity)
        {
            db.Treatments.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Treatment entity)
        {
            db.Treatments.Remove(entity);
            db.SaveChanges();
        }

        public Treatment Find(params object[] keys)
        {
            var t = db.Treatments.Find(keys);
            db.Entry(t).Reference(tr => tr.Doctor).Load();
            db.Entry(t).Reference(tr => tr.Patient).Load();
            db.Entry(t).Collection(tr => tr.Assignments).Load();
            return t;
        }

        public IEnumerable<Treatment> Get(Func<Treatment, bool> condition)
        {
            return db.Treatments.Include(t=>t.Patient).Include(t=>t.Doctor).Where(condition).ToList();
        }

        public IEnumerable<Treatment> GetAll()
        {
            return db.Treatments.Include(t => t.Patient).Include(t => t.Doctor).ToList();
        }

        public void Update(Treatment entity)
        {
            var entry = db.Treatments.Find(entity.TreatmentId);
            db.Entry(entry).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }
}
