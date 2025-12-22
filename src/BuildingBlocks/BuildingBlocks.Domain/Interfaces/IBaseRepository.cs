using System.Linq.Expressions;
using BuildingBlocks.Domain.Models;
using BuildingBlocks.Domain.Specifications;

namespace BuildingBlocks.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetBySpecAsync(Specification<T>? specification = null, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync(Specification<T>? specification = null, params Expression<Func<T, object>>[] includes);
        Task<bool> CheckExistsAsync(Specification<T>? specification = null);
        Task<int> CountAsync(Specification<T>? specification = null);
        Task<List<T>> GetPagedListAsync(Specification<T>? specification = null, int pageNumber = 1, int pageSize = 10,
            string? sortBy = null, bool isAscending = true, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetQueryable();
    }
}
