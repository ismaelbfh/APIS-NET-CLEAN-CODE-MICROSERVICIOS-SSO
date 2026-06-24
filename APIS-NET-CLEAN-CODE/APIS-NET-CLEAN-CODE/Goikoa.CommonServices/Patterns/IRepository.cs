using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.CommonServices.Patterns
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Queryable { get; }
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
