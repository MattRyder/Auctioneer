using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Core.Abstract
{
    /// <summary>
    /// The Repository interface, containing the signatures for required fields and methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepo<T>
    {
        IEnumerable<T> Entities { get; }

        T Find(int id);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
