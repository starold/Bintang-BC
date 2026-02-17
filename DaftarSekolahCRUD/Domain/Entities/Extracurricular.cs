namespace DaftarSekolahCRUD.Domain.Entities
{
    public class Extracurricular
    {
        public Guid Id{get; set;}
        public string Name{get; set; } = string.Empty;
        public ICollection<StudentExtracurricular> StudentExtracurriculars{get; set; } = new List<StudentExtracurricular>();
    }
    
}