using AutoMapper;
using DaftarSekolahCRUD.Application.DTOs.Student;
using DaftarSekolahCRUD.Application.Interfaces;
using DaftarSekolahCRUD.Domain.Entities;
using DaftarSekolahCRUD.Domain.Repositories;
using DaftarSekolahCRUD.Shared;

namespace DaftarSekolahCRUD.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult<StudentResponseDto>> RegisterAsync(RegisterStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);
            student.Id = Guid.NewGuid();

            student.Profile = new StudentProfile
            {
                StudentId = student.Id,
                Address = dto.Address,
                ParentName = dto.ParentName
            };

            student.StudentExtracurriculars = dto.ExtracurricularIds
                .Select(eId => new StudentExtracurricular
                {
                    StudentId = student.Id,
                    ExtracurricularId = eId
                }).ToList();

            await _studentRepository.AddAsync(student);

            var created = await _studentRepository.GetByIdAsync(student.Id);
            var responseDto = _mapper.Map<StudentResponseDto>(created);
            return ServiceResult<StudentResponseDto>.Success(responseDto, "Student registered successfully");
        }

        public async Task<ServiceResult<List<StudentResponseDto>>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            var responseDtos = _mapper.Map<List<StudentResponseDto>>(students);
            return ServiceResult<List<StudentResponseDto>>.Success(responseDtos, "Students retrieved successfully");
        }

        public async Task<ServiceResult<StudentResponseDto>> GetByIdAsync(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return ServiceResult<StudentResponseDto>.Failure("Student not found");

            var responseDto = _mapper.Map<StudentResponseDto>(student);
            return ServiceResult<StudentResponseDto>.Success(responseDto, "Student retrieved successfully");
        }

        public async Task<ServiceResult<StudentResponseDto>> UpdateAsync(Guid id, UpdateStudentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return ServiceResult<StudentResponseDto>.Failure("Student not found");

            student.NIS = dto.NIS;
            student.Name = dto.Name;
            student.ClassRoomId = dto.ClassRoomId;

            if (student.Profile != null)
            {
                student.Profile.Address = dto.Address;
                student.Profile.ParentName = dto.ParentName;
            }
            else
            {
                student.Profile = new StudentProfile
                {
                    StudentId = student.Id,
                    Address = dto.Address,
                    ParentName = dto.ParentName
                };
            }

            student.StudentExtracurriculars = dto.ExtracurricularIds
                .Select(eId => new StudentExtracurricular
                {
                    StudentId = student.Id,
                    ExtracurricularId = eId
                }).ToList();

            await _studentRepository.UpdateAsync(student);

            var updated = await _studentRepository.GetByIdAsync(id);
            var responseDto = _mapper.Map<StudentResponseDto>(updated);
            return ServiceResult<StudentResponseDto>.Success(responseDto, "Student updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            var result = await _studentRepository.DeleteAsync(id);
            if (!result)
                return ServiceResult<bool>.Failure("Student not found");

            return ServiceResult<bool>.Success(true, "Student deleted successfully");
        }
    }
}
