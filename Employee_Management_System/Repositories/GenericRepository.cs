using Employee_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Employee_Management_System.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public IQueryable<TEntity> Query(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            return query;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = Query(include, disableTracking);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = Query(include, disableTracking);

            query = query.Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> GetByIdAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
            {
                throw new ArgumentException("Key values must be provided.", nameof(keyValues));
            }

            return await _dbSet.FindAsync(keyValues);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
