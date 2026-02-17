using DaftarSekolahCRUD.Application.DTOs.Auth;
using DaftarSekolahCRUD.Application.Interfaces;
using DaftarSekolahCRUD.Domain.Entities;
using DaftarSekolahCRUD.Domain.Repositories;
using DaftarSekolahCRUD.Infrastructure.Authentication;
using DaftarSekolahCRUD.Shared;

namespace DaftarSekolahCRUD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtService _jwtService;

        public AuthService(IAuthRepository authRepository, JwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterUserDto dto)
        {
            if (await _authRepository.UserExistsAsync(dto.Username))
                return ServiceResult<string>.Failure("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _authRepository.AddUserAsync(user);
            return ServiceResult<string>.Success("User registered successfully", "Registration successful");
        }

        public async Task<ServiceResult<string>> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                return ServiceResult<string>.Failure("Invalid username or password");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ServiceResult<string>.Failure("Invalid username or password");

            var token = _jwtService.GenerateToken(user);
            return ServiceResult<string>.Success(token, "Login successful");
        }
    }
}
