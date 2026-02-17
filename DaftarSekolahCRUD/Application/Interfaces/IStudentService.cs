using DaftarSekolahCRUD.Application.DTOs.Student;
using DaftarSekolahCRUD.Shared;

namespace DaftarSekolahCRUD.Application.Interfaces
{
    public interface IStudentService
    {
        Task<ServiceResult<StudentResponseDto>> RegisterAsync(RegisterStudentDto dto);
        Task<ServiceResult<List<StudentResponseDto>>> GetAllAsync();
        Task<ServiceResult<StudentResponseDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<StudentResponseDto>> UpdateAsync(Guid id, UpdateStudentDto dto);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}