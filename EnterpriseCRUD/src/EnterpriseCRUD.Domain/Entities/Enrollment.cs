namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Enrollment entity — demonstrates multiple One-to-Many relationships.
/// Links Student, Subject, and ClassRoom.
/// </summary>
public class Enrollment : BaseEntity
{
    // Foreign Keys
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public Guid ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public string? Grade { get; set; }
}
