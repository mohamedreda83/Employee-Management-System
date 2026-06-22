using System.Linq.Expressions;

namespace Employee_Management_System.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(params object[] keyValues);
        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true);
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<int> SaveChangesAsync();
        IQueryable<TEntity> Query(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool disableTracking = true);
    }
}