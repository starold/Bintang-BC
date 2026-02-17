namespace DaftarSekolahCRUD.Domain.Entities
{
    public class Student
    {
        public Guid Id {get; set;}
        public string NIS {get; set; } = string.Empty;
        public string Name{get; set; } = string.Empty;
        public Guid ClassRoomId{get; set;}
        public ClassRoom ClassRoom{get; set; } = null!;
        public StudentProfile Profile {get; set; } = null!;
        public ICollection<StudentExtracurricular> StudentExtracurriculars{get; set; } = new List<StudentExtracurricular>();
    
    }
}

