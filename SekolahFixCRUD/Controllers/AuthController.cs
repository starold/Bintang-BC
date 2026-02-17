using Microsoft.AspNetCore.Mvc;
using SekolahFixCRUD.DTOs.Auth;
using SekolahFixCRUD.Interfaces;

namespace SekolahFixCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        _logger.LogInformation("Registering user: {Email}", registerDto.Email);
        var result = await _authService.RegisterAsync(registerDto);
        
        if (!result.Success)
            return StatusCode((int)result.StatusCode, result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for user: {Email}", loginDto.Email);
        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
            return StatusCode((int)result.StatusCode, result);

        return Ok(result);
    }
}
