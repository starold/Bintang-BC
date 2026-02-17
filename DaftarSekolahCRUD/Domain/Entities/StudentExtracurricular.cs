namespace DaftarSekolahCRUD.Domain.Entities
{
    public class StudentExtracurricular
    {
        public Guid StudentId{get; set;}
        public Student Student{get; set;} = null!;
        
        public Guid ExtracurricularId{get; set;}
        public Extracurricular Extracurricular{get; set;} = null!;
    }
}