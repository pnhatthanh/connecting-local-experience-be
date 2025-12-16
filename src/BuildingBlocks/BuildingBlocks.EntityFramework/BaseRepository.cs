using System.Linq.Expressions;
using BuildingBlocks.Domain.Interfaces;
using BuildingBlocks.Domain.Models;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;


namespace BuildingBlocks.EntityFramework
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly BaseDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public BaseRepository(BaseDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public Task<T?> GetBySpecAsync(Specification<T>? specification = null, params Expression<Func<T, object>>[] includes)
        {
            return _dbSet.ApplySpecification(specification)
                          .ApplyInclude(includes)
                          .FirstOrDefaultAsync();
        }
        public Task<List<T>> GetAllAsync(Specification<T>? specification = null, params Expression<Func<T, object>>[] includes)
        {
            return _dbSet.AsNoTracking()
                          .ApplySpecification(specification)
                          .ApplyInclude(includes)
                          .ToListAsync();
        }
        public Task<bool> CheckExistsAsync(Specification<T>? specification = null)
        {
            return _dbSet.ApplySpecification(specification)
                          .AnyAsync();
        }
        public Task<int> CountAsync(Specification<T>? specification = null)
        {
            return _dbSet.ApplySpecification(specification)
                          .CountAsync();
        }
        public Task<List<T>> GetPagedListAsync(Specification<T>? specification = null, int pageNumber = 1, int pageSize = 10,
            string? sortBy = null, bool isAscending = true, params Expression<Func<T, object>>[] includes)
        {
            return _dbSet.AsNoTracking()
                        .ApplySpecification(specification)
                        .ApplySorting(sortBy, isAscending)
                        .ApplyPaging(pageNumber, pageSize)
                        .ApplyInclude(includes)
                        .ToListAsync();
        }
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
    internal static class ExternalRepository
    {
        public static IQueryable<T> ApplyInclude<T>(this IQueryable<T> query, Expression<Func<T, object>>[] includes) where T : class
        {
            return includes == null ? query : includes.Aggregate(query, (current, include) => current.Include(include));
        }
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Expression<Func<T, bool>>? expression)
        {
            return expression == null ? query : query.Where(expression);
        }
        public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, Specification<T>? specification) where T : BaseEntity
        {
            if (specification == null)
                return query;
            return query.ApplyFilter(specification.ToExpression());
        }
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);
        }
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;
            var property = typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                return query;
            return isAscending ? query.OrderBy(sortBy) : query.OrderBy(sortBy + " descending");
        }
    }
}
