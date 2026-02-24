using System.Linq.Expressions;
using EnterpriseCRUD.Domain.Entities;

namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface supporting dynamic queries.
/// </summary>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, params string[] includes);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        string? sortField = null,
        string sortOrder = "asc",
        string[]? includes = null);

    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        string[]? includes = null);

    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Guid id);
}
