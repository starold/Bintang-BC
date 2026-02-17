using DaftarSekolahCRUD.Domain.Entities;

namespace DaftarSekolahCRUD.Domain.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
    }
}
