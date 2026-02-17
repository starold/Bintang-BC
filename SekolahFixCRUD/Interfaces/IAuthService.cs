using SekolahFixCRUD.Common;
using SekolahFixCRUD.DTOs.Auth;

namespace SekolahFixCRUD.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto);
}
