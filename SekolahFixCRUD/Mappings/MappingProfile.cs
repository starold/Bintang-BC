using AutoMapper;
using SekolahFixCRUD.DTOs.Auth;
using SekolahFixCRUD.DTOs.Course;
using SekolahFixCRUD.DTOs.Student;
using SekolahFixCRUD.DTOs.Teacher;
using SekolahFixCRUD.Entities;

namespace SekolahFixCRUD.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth
        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        // Course
        CreateMap<Course, CourseResponseDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName));
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CourseUpdateDto, Course>();

        // Teacher
        CreateMap<Teacher, TeacherResponseDto>();
        CreateMap<TeacherCreateDto, Teacher>();

        // Student
        CreateMap<StudentProfile, StudentProfileDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
        CreateMap<StudentUpdateDto, StudentProfile>();
    }
}
