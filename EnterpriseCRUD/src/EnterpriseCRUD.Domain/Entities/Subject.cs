namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Subject entity — demonstrates Many-to-Many with Teacher,
/// and One-to-Many with Enrollment.
/// </summary>
public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Credits { get; set; }

    // Navigation: Many-to-Many Teacher ↔ Subject
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

    // Navigation: One Subject → Many Enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
