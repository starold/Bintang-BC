using DaftarSekolahCRUD.Domain.Entities;

namespace DaftarSekolahCRUD.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(Guid id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task<bool> DeleteAsync(Guid id);
    }
}
