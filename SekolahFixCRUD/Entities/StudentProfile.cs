namespace SekolahFixCRUD.Entities;


public class StudentProfile : BaseEntity
{
    public string UserId { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = default!;
    public string? PhoneNumber { get; set; }

    public bool IsApproved { get; set; }
    public string? ApprovalStatus { get; set; } // Pending, Approved, Rejected
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedByAdminId { get; set; }

    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

