using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(Func<T, bool> condition);
        T Find(params object[] keys);
        void Update(T entity);
        void Add(T entity);
        void Delete(T entity);
    }
}
