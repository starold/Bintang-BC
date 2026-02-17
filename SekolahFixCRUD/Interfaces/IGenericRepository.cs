using System.Linq.Expressions;

namespace SekolahFixCRUD.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    // Pagination
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    
    // Soft Delete & Restore
    Task SoftDeleteAsync(int id);
    Task RestoreAsync(int id);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
}
