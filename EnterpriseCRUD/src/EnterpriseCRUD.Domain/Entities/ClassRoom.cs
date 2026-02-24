namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// ClassRoom entity — demonstrates Many-to-One with Teacher.
/// </summary>
public class ClassRoom : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // Foreign Key: Many-to-One → Teacher
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    // Navigation: One ClassRoom → Many Enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
