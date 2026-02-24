using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRUD.Infrastructure.Services;

/// <summary>
/// Authentication service using ASP.NET Identity + JWT.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthResult { Success = false, Errors = new List<string> { "Invalid email or password." } };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            return new AuthResult { Success = false, Errors = new List<string> { "Invalid email or password." } };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenGenerator.GenerateToken(user, roles);

        return new AuthResult
        {
            Success = true,
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? role = null)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            return new AuthResult { Success = false, Errors = new List<string> { "Email already registered." } };
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return new AuthResult
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        // Assign role
        var assignRole = role ?? "User";
        await _userManager.AddToRoleAsync(user, assignRole);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenGenerator.GenerateToken(user, roles);

        return new AuthResult
        {
            Success = true,
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles.ToList()
        };
    }
}
