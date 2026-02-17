namespace SekolahFixCRUD.Entities;


public class Teacher : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Department { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}

