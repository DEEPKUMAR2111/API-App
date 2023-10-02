
using Data.DataAccess.Data.Interface.IData;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.DataAccess.Data.Interface
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext _db;
        private DbSet<T> dbset;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbset = _db.Set<T>();
        }

        public void Add(T item)
        {
            dbset.Add(item);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate=null, string? includeProperties=null)
        {
            IQueryable<T> query = dbset;
            if(predicate != null)
            {
                query = query.Where(predicate).AsNoTracking();
            }
            if(includeProperties!=null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp).AsNoTracking();
                }
            }
            return query.AsNoTracking().ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {

            IQueryable<T> query = dbset;
            query = query.Where(predicate).AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp).AsNoTracking();
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T item)
        {
            dbset.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> item)
        {
            dbset.RemoveRange(item);
        }
    }
}
