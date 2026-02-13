namespace DaftarSekolahCRUD.Domain.Entities
{
    public class StudentProfile
    {
        public Guid StudentId{get; set;}
        public string Address{get; set;}
        public string ParentName{get; set;}
        public Student Student{get; set;}
    }
}