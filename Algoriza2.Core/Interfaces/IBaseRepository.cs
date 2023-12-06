using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T GetById(int id);
        
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        T Find(Expression<Func<T, bool>> match, string[] includes = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] includes);



        T Add(T entity);
        T Update(T entity);
        // void Delete(int id);
        int Count();


    }


}
