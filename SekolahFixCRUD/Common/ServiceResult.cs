using System.Net;

namespace SekolahFixCRUD.Common;


public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public string? ErrorCode { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }
    public List<string> Errors { get; } = new();
    public T? Data { get; private set; }

    private ServiceResult() { }

    public static ServiceResult<T> Succeeded(T? data, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };

    public static ServiceResult<T> Ok(T? data, string? message = null)
        => Succeeded(data, message, HttpStatusCode.OK);

    public static ServiceResult<T> Created(T? data, string? message = null)
        => Succeeded(data, message, HttpStatusCode.Created);

    public static ServiceResult<T> NotFound(string message, string? errorCode = null)
        => new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode ?? "NOT_FOUND",
            StatusCode = HttpStatusCode.NotFound
        };

    public static ServiceResult<T> Failure(string message, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode ?? "ERROR",
            StatusCode = statusCode
        };

    public static ServiceResult<T> ValidationFailure(IEnumerable<string> @errors, string? message = null)
    {
        var result = new ServiceResult<T>
        {
            Success = false,
            Message = message ?? "Validation failed.",
            ErrorCode = "VALIDATION_ERROR",
            StatusCode = HttpStatusCode.BadRequest
        };

        result.Errors.AddRange(@errors);
        return result;
    }
}
