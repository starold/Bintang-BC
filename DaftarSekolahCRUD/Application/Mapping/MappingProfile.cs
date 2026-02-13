using AutoMapper;
using DaftarSekolahCRUD.Application.DTOs;
using DaftarSekolahCRUD.Domain.Entities;

namespace DaftarSekolahCRUD.Application.Domain
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateStudentDto, Student>();
        }
    }
}

