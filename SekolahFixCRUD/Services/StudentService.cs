using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SekolahFixCRUD.Common;
using SekolahFixCRUD.DTOs.Course;
using SekolahFixCRUD.DTOs.Student;
using SekolahFixCRUD.Entities;
using SekolahFixCRUD.Interfaces;

namespace SekolahFixCRUD.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository<StudentProfile> _profileRepo;
    private readonly IGenericRepository<Course> _courseRepo;
    private readonly IGenericRepository<StudentCourse> _studentCourseRepo;
    private readonly IMapper _mapper;

    public StudentService(
        IGenericRepository<StudentProfile> profileRepo,
        IGenericRepository<Course> courseRepo,
        IGenericRepository<StudentCourse> studentCourseRepo,
        IMapper mapper)
    {
        _profileRepo = profileRepo;
        _courseRepo = courseRepo;
        _studentCourseRepo = studentCourseRepo;
        _mapper = mapper;
    }

    public async Task<ServiceResult<StudentProfileDto>> GetProfileAsync(string userId)
    {
        var profile = (await _profileRepo.FindAsync(p => p.UserId == userId)).FirstOrDefault();
        if (profile == null) return ServiceResult<StudentProfileDto>.NotFound("Profile not found");

        return ServiceResult<StudentProfileDto>.Succeeded(_mapper.Map<StudentProfileDto>(profile));
    }

    public async Task<ServiceResult<StudentProfileDto>> UpdateProfileAsync(string userId, StudentUpdateDto updateDto)
    {
        var profile = (await _profileRepo.FindAsync(p => p.UserId == userId)).FirstOrDefault();
        if (profile == null) return ServiceResult<StudentProfileDto>.NotFound("Profile not found");

        _mapper.Map(updateDto, profile);
        _profileRepo.Update(profile);
        await _profileRepo.SaveChangesAsync();

        return ServiceResult<StudentProfileDto>.Succeeded(_mapper.Map<StudentProfileDto>(profile), "Profile updated successfully");
    }

    public async Task<ServiceResult<PaginatedResultDto<CourseResponseDto>>> GetAvailableCoursesAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _courseRepo.GetPagedAsync(pageNumber, pageSize);
        var dtos = _mapper.Map<IEnumerable<CourseResponseDto>>(items);

        return ServiceResult<PaginatedResultDto<CourseResponseDto>>.Succeeded(new PaginatedResultDto<CourseResponseDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<bool>> EnrollCoursesAsync(string userId, StudentEnrollmentDto enrollmentDto)
    {
        var profile = (await _profileRepo.FindAsync(p => p.UserId == userId)).FirstOrDefault();
        if (profile == null) return ServiceResult<bool>.Failure("Student profile not found");
        
        if (!profile.IsApproved) 
            return ServiceResult<bool>.Failure("Registration pending approval. You cannot enroll in courses yet.", "PENDING_APPROVAL", System.Net.HttpStatusCode.Forbidden);

        foreach (var courseId in enrollmentDto.CourseIds)
        {
            if (await _studentCourseRepo.ExistsAsync(sc => sc.StudentProfileId == profile.Id && sc.CourseId == courseId))
                continue;

            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null) continue;

            await _studentCourseRepo.AddAsync(new StudentCourse
            {
                StudentProfileId = profile.Id,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            });
        }

        await _studentCourseRepo.SaveChangesAsync();
        return ServiceResult<bool>.Succeeded(true, "Enrollment successful");
    }
}
