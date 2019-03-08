using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class NurseRepository : IRepository<Nurse>
    {
        private HospitalDbContext db = HospitalDbContext.Create();
        public void Add(Nurse entity)
        {
            db.Nurses.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Nurse entity)
        {
            db.Nurses.Remove(entity);
            db.SaveChanges();
        }

        public Nurse Find(params object[] keys)
        {
            return db.Nurses.Find(keys);
        }

        public IEnumerable<Nurse> Get(Func<Nurse, bool> condition)
        {
            return db.Nurses.Where(condition).ToList();
        }

        public IEnumerable<Nurse> GetAll()
        {
            return db.Nurses.ToList();
        }

        public void Update(Nurse entity)
        {
            var entry = db.Nurses.Find(entity.UserId);
            db.Entry(entry).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }
}
