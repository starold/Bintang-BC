namespace DaftarSekolahCRUD.Domain.Entities
{
    public class StudentProfile
    {
        public Guid StudentId{get; set;}
        public string Address{get; set;} = string.Empty;
        public string ParentName{get; set;} = string.Empty;
        public Student Student{get; set;} = null!;
    }
}