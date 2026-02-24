using System.Linq.Expressions;
using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCRUD.Infrastructure.Repositories;

/// <summary>
/// Generic repository with dynamic sorting, filtering, pagination, and eager loading.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, params string[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? filter = null,
        string? sortField = null,
        string sortOrder = "asc",
        string[]? includes = null)
    {
        IQueryable<T> query = _dbSet;

        // Apply includes
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        // Apply filter
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrEmpty(sortField))
        {
            query = ApplySort(query, sortField, sortOrder);
        }

        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        string[]? includes = null)
    {
        IQueryable<T> query = _dbSet;

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }

    /// <summary>Apply dynamic sorting using reflection.</summary>
    private static IQueryable<T> ApplySort(IQueryable<T> query, string sortField, string sortOrder)
    {
        var property = typeof(T).GetProperty(sortField,
            System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (property == null) return query.OrderByDescending(e => e.CreatedAt);

        var param = Expression.Parameter(typeof(T), "x");
        var propAccess = Expression.Property(param, property);
        var keySelector = Expression.Lambda(propAccess, param);

        var methodName = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending"
            : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(keySelector));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}
