namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Join entity for Many-to-Many: Teacher ↔ Subject.
/// </summary>
public class TeacherSubject
{
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}
