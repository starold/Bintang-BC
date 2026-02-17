using AutoMapper;
using SekolahFixCRUD.Common;
using SekolahFixCRUD.DTOs.Course;
using SekolahFixCRUD.DTOs.Student;
using SekolahFixCRUD.DTOs.Teacher;
using SekolahFixCRUD.Entities;
using SekolahFixCRUD.Interfaces;

namespace SekolahFixCRUD.Services;

public class AdminService : IAdminService
{
    private readonly IGenericRepository<Course> _courseRepo;
    private readonly IGenericRepository<Teacher> _teacherRepo;
    private readonly IGenericRepository<StudentProfile> _studentRepo;
    private readonly IMapper _mapper;

    public AdminService(
        IGenericRepository<Course> courseRepo,
        IGenericRepository<Teacher> teacherRepo,
        IGenericRepository<StudentProfile> studentRepo,
        IMapper mapper)
    {
        _courseRepo = courseRepo;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _mapper = mapper;
    }

    // --- Course CRUD ---
    public async Task<ServiceResult<CourseResponseDto>> CreateCourseAsync(CourseCreateDto createDto)
    {
        var course = _mapper.Map<Course>(createDto);
        await _courseRepo.AddAsync(course);
        await _courseRepo.SaveChangesAsync();
        return ServiceResult<CourseResponseDto>.Succeeded(_mapper.Map<CourseResponseDto>(course), "Course created successfully");
    }

    public async Task<ServiceResult<CourseResponseDto>> UpdateCourseAsync(int id, CourseUpdateDto updateDto)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null) return ServiceResult<CourseResponseDto>.NotFound("Course not found");

        _mapper.Map(updateDto, course);
        _courseRepo.Update(course);
        await _courseRepo.SaveChangesAsync();
        return ServiceResult<CourseResponseDto>.Succeeded(_mapper.Map<CourseResponseDto>(course), "Course updated successfully");
    }

    public async Task<ServiceResult<bool>> DeleteCourseAsync(int id)
    {
        await _courseRepo.SoftDeleteAsync(id);
        await _courseRepo.SaveChangesAsync();
        return ServiceResult<bool>.Succeeded(true, "Course soft-deleted successfully");
    }

    public async Task<ServiceResult<bool>> RestoreCourseAsync(int id)
    {
        await _courseRepo.RestoreAsync(id);
        await _courseRepo.SaveChangesAsync();
        return ServiceResult<bool>.Succeeded(true, "Course restored successfully");
    }

    public async Task<ServiceResult<PaginatedResultDto<CourseResponseDto>>> GetAllCoursesAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _courseRepo.GetPagedAsync(pageNumber, pageSize);
        return ServiceResult<PaginatedResultDto<CourseResponseDto>>.Succeeded(new PaginatedResultDto<CourseResponseDto>
        {
            Items = _mapper.Map<IEnumerable<CourseResponseDto>>(items),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    // --- Teacher CRUD ---
    public async Task<ServiceResult<TeacherResponseDto>> CreateTeacherAsync(TeacherCreateDto createDto)
    {
        var teacher = _mapper.Map<Teacher>(createDto);
        await _teacherRepo.AddAsync(teacher);
        await _teacherRepo.SaveChangesAsync();
        return ServiceResult<TeacherResponseDto>.Succeeded(_mapper.Map<TeacherResponseDto>(teacher), "Teacher created successfully");
    }

    public async Task<ServiceResult<PaginatedResultDto<TeacherResponseDto>>> GetAllTeachersAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _teacherRepo.GetPagedAsync(pageNumber, pageSize);
        return ServiceResult<PaginatedResultDto<TeacherResponseDto>>.Succeeded(new PaginatedResultDto<TeacherResponseDto>
        {
            Items = _mapper.Map<IEnumerable<TeacherResponseDto>>(items),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    // --- Approval Workflow ---
    public async Task<ServiceResult<bool>> ApproveStudentAsync(int studentProfileId, string adminId)
    {
        var student = await _studentRepo.GetByIdAsync(studentProfileId);
        if (student == null) return ServiceResult<bool>.NotFound("Student not found");

        student.IsApproved = true;
        student.ApprovalStatus = "Approved";
        student.ApprovalDate = DateTime.UtcNow;
        student.ApprovedByAdminId = adminId;

        _studentRepo.Update(student);
        await _studentRepo.SaveChangesAsync();
        return ServiceResult<bool>.Succeeded(true, "Student approved successfully");
    }

    public async Task<ServiceResult<bool>> RejectStudentAsync(int studentProfileId, string adminId)
    {
        var student = await _studentRepo.GetByIdAsync(studentProfileId);
        if (student == null) return ServiceResult<bool>.NotFound("Student not found");

        student.IsApproved = false;
        student.ApprovalStatus = "Rejected";
        student.ApprovalDate = DateTime.UtcNow;
        student.ApprovedByAdminId = adminId;

        _studentRepo.Update(student);
        await _studentRepo.SaveChangesAsync();
        return ServiceResult<bool>.Succeeded(true, "Student rejected");
    }

    public async Task<ServiceResult<PaginatedResultDto<StudentProfileDto>>> GetAllStudentsAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _studentRepo.GetPagedAsync(pageNumber, pageSize);
        return ServiceResult<PaginatedResultDto<StudentProfileDto>>.Succeeded(new PaginatedResultDto<StudentProfileDto>
        {
            Items = _mapper.Map<IEnumerable<StudentProfileDto>>(items),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}
