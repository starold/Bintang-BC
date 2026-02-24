namespace EnterpriseCRUD.Application.Common.Models;

/// <summary>
/// Strong-typed result for paged list operations.
/// Prevents anonymous type access issues across assemblies.
/// </summary>
public class PagedResult
{
    public IEnumerable<object> Data { get; set; } = new List<object>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
