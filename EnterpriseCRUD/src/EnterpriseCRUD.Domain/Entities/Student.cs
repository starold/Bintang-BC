using EnterpriseCRUD.Domain.Enums;

namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Student entity — demonstrates One-to-Many with Enrollment.
/// </summary>
public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    // Navigation: One Student → Many Enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
