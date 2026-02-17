namespace SekolahFixCRUD.Entities;


public class Course : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public int Credits { get; set; }

    public int TeacherId { get; set; }
    public virtual Teacher Teacher { get; set; } = default!;

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

