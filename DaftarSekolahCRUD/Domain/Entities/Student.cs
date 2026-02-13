namespace DaftarSekolahCRUD.Domain.Entities
{
    public class Student
    {
        public Guid Id {get; set;}
        public string NIS {get; set;}
        public string Name{get; set;}
        public Guid ClassRoomId{get; set;}
        public ClassRoom ClassRoom{get; set;}
        public StudentProfile Profile {get; set;}
        public ICollection<StudentExtracurricular> StudentExtracurriculars{get; set;}
    
    }
}

