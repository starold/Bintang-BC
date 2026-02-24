using EnterpriseCRUD.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.API.Controllers;

/// <summary>
/// Authentication endpoints: login and register.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
            return Unauthorized(new { success = false, errors = result.Errors });

        return Ok(new
        {
            success = true,
            token = result.Token,
            email = result.Email,
            fullName = result.FullName,
            roles = result.Roles
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(
            request.Email, request.Password, request.FullName, request.Role);

        if (!result.Success)
            return BadRequest(new { success = false, errors = result.Errors });

        return Ok(new
        {
            success = true,
            token = result.Token,
            email = result.Email,
            fullName = result.FullName,
            roles = result.Roles
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Role { get; set; }
}
