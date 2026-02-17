namespace SekolahFixCRUD.DTOs.Course
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public int Credits { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = default!;
    }

    public class CourseCreateDto
    {
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public int Credits { get; set; }
        public int TeacherId { get; set; }
    }

    public class CourseUpdateDto : CourseCreateDto { }
}

namespace SekolahFixCRUD.DTOs.Teacher
{
    public class TeacherResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Department { get; set; }
    }

    public class TeacherCreateDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Department { get; set; }
    }
}
