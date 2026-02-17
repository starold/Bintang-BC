using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SekolahFixCRUD.Common;
using SekolahFixCRUD.Data;
using SekolahFixCRUD.DTOs.Auth;
using SekolahFixCRUD.Entities;
using SekolahFixCRUD.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SekolahFixCRUD.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IMapper mapper,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        var user = _mapper.Map<ApplicationUser>(registerDto);
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Registration failed", 
                "REGISTRATION_ERROR", 
                System.Net.HttpStatusCode.BadRequest)
                .WithErrors(result.Errors.Select(e => e.Description));
        }

        // Assign Student Role
        if (!await _roleManager.RoleExistsAsync("Student"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Student"));
        }
        await _userManager.AddToRoleAsync(user, "Student");

        // Create Student Profile
        var profile = new StudentProfile
        {
            UserId = user.Id,
            FullName = registerDto.FullName,
            Address = registerDto.Address,
            DateOfBirth = registerDto.DateOfBirth,
            PhoneNumber = registerDto.PhoneNumber,
            IsApproved = false,
            ApprovalStatus = "Pending"
        };
        _context.StudentProfiles.Add(profile);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user, "Student");

        return ServiceResult<AuthResponseDto>.Succeeded(new AuthResponseDto
        {
            Email = user.Email!,
            Token = token,
            Role = "Student"
        }, "Registration successful. Your profile is pending approval.");
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return ServiceResult<AuthResponseDto>.Failure("Invalid email or password", "INVALID_CREDENTIALS");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            return ServiceResult<AuthResponseDto>.Failure("Invalid email or password", "INVALID_CREDENTIALS");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";
        var token = GenerateJwtToken(user, role);

        return ServiceResult<AuthResponseDto>.Succeeded(new AuthResponseDto
        {
            Email = user.Email!,
            Token = token,
            Role = role
        }, "Login successful");
    }

    private string GenerateJwtToken(ApplicationUser user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.Email!),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? "YourVerySecretKeyThatIsAtLeast32CharsLong!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(7);

        var token = new JwtSecurityToken(
            _configuration["JWT:Issuer"],
            _configuration["JWT:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Extension to help with ServiceResult error collection
public static class ServiceResultExtensions
{
    public static ServiceResult<T> WithErrors<T>(this ServiceResult<T> result, IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            result.Errors.Add(error);
        }
        return result;
    }
}
