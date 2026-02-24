using System.Net;

namespace EnterpriseCRUD.Application.Common.Models;

/// <summary>
/// Generic service result wrapper for consistent API responses.
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public static ServiceResult<T> SuccessResult(T data, string? message = null)
    {
        return new ServiceResult<T> { Success = true, Data = data, Message = message };
    }

    public static ServiceResult<T> ErrorResult(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T> { Success = false, Errors = new List<string> { error }, StatusCode = statusCode };
    }

    public static ServiceResult<T> ErrorResult(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T> { Success = false, Errors = errors, StatusCode = statusCode };
    }
}

/// <summary>
/// Paginated result wrapper.
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
}
