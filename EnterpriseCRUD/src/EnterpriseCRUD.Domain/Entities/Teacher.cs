namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Teacher entity — demonstrates One-to-Many with ClassRoom,
/// and Many-to-Many with Subject via TeacherSubject join.
/// </summary>
public class Teacher : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;

    // Navigation: One Teacher → Many ClassRooms (homeroom)
    public ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();

    // Navigation: Many-to-Many Teacher ↔ Subject
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}
