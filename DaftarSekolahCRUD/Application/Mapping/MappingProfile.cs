using AutoMapper;
using DaftarSekolahCRUD.Application.DTOs.Student;
using DaftarSekolahCRUD.Domain.Entities;

namespace DaftarSekolahCRUD.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterStudentDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClassRoom, opt => opt.Ignore())
                .ForMember(dest => dest.Profile, opt => opt.Ignore())
                .ForMember(dest => dest.StudentExtracurriculars, opt => opt.Ignore());

            CreateMap<Student, StudentResponseDto>()
                .ForMember(dest => dest.ClassName,
                    opt => opt.MapFrom(src => src.ClassRoom != null ? src.ClassRoom.Name : string.Empty))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Address : string.Empty))
                .ForMember(dest => dest.ParentName,
                    opt => opt.MapFrom(src => src.Profile != null ? src.Profile.ParentName : string.Empty))
                .ForMember(dest => dest.Extracurriculars,
                    opt => opt.MapFrom(src => src.StudentExtracurriculars != null
                        ? src.StudentExtracurriculars
                            .Where(se => se.Extracurricular != null)
                            .Select(se => se.Extracurricular.Name)
                            .ToList()
                        : new List<string>()));
        }
    }
}
