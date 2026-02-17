using DaftarSekolahCRUD.Application.DTOs.Auth;
using DaftarSekolahCRUD.Shared;

namespace DaftarSekolahCRUD.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterUserDto dto);
        Task<ServiceResult<string>> LoginAsync(LoginDto dto);
    }
}
