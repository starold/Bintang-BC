namespace DaftarSekolahCRUD.Application.DTOs.Student
{
    public class StudentResponseDto
    {
        public Guid Id { get; set; }
        public string NIS { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public List<string> Extracurriculars { get; set; } = new();
    }
}
