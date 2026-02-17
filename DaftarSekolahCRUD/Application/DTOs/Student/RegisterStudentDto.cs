namespace DaftarSekolahCRUD.Application.DTOs.Student
{
    public class RegisterStudentDto
    {
        public string NIS { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid ClassRoomId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public List<Guid> ExtracurricularIds { get; set; } = new();
    }
}
