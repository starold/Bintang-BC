namespace SekolahFixCRUD.DTOs.Student;

public class StudentProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? ApprovalStatus { get; set; }
    public bool IsApproved { get; set; }
}

public class StudentUpdateDto
{
    public string FullName { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = default!;
    public string? PhoneNumber { get; set; }
}

public class StudentEnrollmentDto
{
    public List<int> CourseIds { get; set; } = new();
}

public class PaginatedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
