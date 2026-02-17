using SekolahFixCRUD.Common;
using SekolahFixCRUD.DTOs.Course;
using SekolahFixCRUD.DTOs.Student;
using SekolahFixCRUD.DTOs.Teacher;

namespace SekolahFixCRUD.Interfaces;

public interface IStudentService
{
    Task<ServiceResult<StudentProfileDto>> GetProfileAsync(string userId);
    Task<ServiceResult<StudentProfileDto>> UpdateProfileAsync(string userId, StudentUpdateDto updateDto);
    Task<ServiceResult<PaginatedResultDto<CourseResponseDto>>> GetAvailableCoursesAsync(int pageNumber, int pageSize);
    Task<ServiceResult<bool>> EnrollCoursesAsync(string userId, StudentEnrollmentDto enrollmentDto);
}

public interface IAdminService
{
    // Course CRUD
    Task<ServiceResult<CourseResponseDto>> CreateCourseAsync(CourseCreateDto createDto);
    Task<ServiceResult<CourseResponseDto>> UpdateCourseAsync(int id, CourseUpdateDto updateDto);
    Task<ServiceResult<bool>> DeleteCourseAsync(int id);
    Task<ServiceResult<bool>> RestoreCourseAsync(int id);
    Task<ServiceResult<PaginatedResultDto<CourseResponseDto>>> GetAllCoursesAsync(int pageNumber, int pageSize);

    // Teacher CRUD
    Task<ServiceResult<TeacherResponseDto>> CreateTeacherAsync(TeacherCreateDto createDto);
    Task<ServiceResult<PaginatedResultDto<TeacherResponseDto>>> GetAllTeachersAsync(int pageNumber, int pageSize);

    // Approval Workflow
    Task<ServiceResult<bool>> ApproveStudentAsync(int studentProfileId, string adminId);
    Task<ServiceResult<bool>> RejectStudentAsync(int studentProfileId, string adminId);
    Task<ServiceResult<PaginatedResultDto<StudentProfileDto>>> GetAllStudentsAsync(int pageNumber, int pageSize);
}
