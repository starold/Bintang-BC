using EnterpriseCRUD.Application.Common.Models;

namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Interface for the dynamic CRUD service.
/// Handles CRUD operations for any registered entity by name.
/// </summary>
public interface ICrudService
{
    Task<PagedResult> GetPagedAsync(string entityName, int page, int pageSize,
        string? sortField, string? sortOrder, string? search,
        Dictionary<string, string>? filters);

    Task<object?> GetByIdAsync(string entityName, Guid id);
    Task<object> CreateAsync(string entityName, Dictionary<string, object?> data);
    Task<object?> UpdateAsync(string entityName, Guid id, Dictionary<string, object?> data);
    Task<bool> DeleteAsync(string entityName, Guid id);
}
