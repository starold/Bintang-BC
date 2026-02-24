namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? role = null);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
