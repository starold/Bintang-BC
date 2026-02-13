using DaftarSekolahCRUD.Application.DTOs;
using DaftarSekolahCRUD.Application.Services;
using DaftarSekolahCRUD.Domain.Entities;

namespace DaftarSekolahCRUD.Application.Interfaces
{
    public interface IStudentService
    {
        Task<ServiceResult<Guid>> CreateAsync(CreateStudentDto dto);
        Task<ServiceResult<List<Student>>> GetAllAsync();
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}