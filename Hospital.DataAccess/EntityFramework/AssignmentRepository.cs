using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class AssignmentRepository : IRepository<Assignment>
    {
        private HospitalDbContext db = HospitalDbContext.Create();
        public void Add(Assignment entity)
        {
            db.Assignments.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Assignment entity)
        {
            db.Assignments.Remove(entity);
            db.SaveChanges();
        }

        public Assignment Find(params object[] keys)
        {
            var assignment= db.Assignments.Find(keys);
            db.Entry(assignment).Reference(a => a.Treatment).Load();
            db.Entry(assignment.Treatment).Reference(a => a.Patient).Load();
            db.Entry(assignment).Reference(a => a.Doctor).Load();
            db.Entry(assignment).Reference(a => a.Nurse).Load();
            return assignment;
        }

        public IEnumerable<Assignment> Get(Func<Assignment, bool> condition)
        {
            return db.Assignments
                .Include(a=>a.Treatment)
                .Include(a=>a.Treatment.Patient)
                .Where(condition).ToList();
        }

        public IEnumerable<Assignment> GetAll()
        {
            return db.Assignments.ToList();
        }

        public void Update(Assignment entity)
        {
            var entry = db.Assignments.Find(entity.AssignmentId);
            db.Entry(entry).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }
}
