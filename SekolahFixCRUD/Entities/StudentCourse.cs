namespace SekolahFixCRUD.Entities;


public class StudentCourse : BaseEntity
{
    public int StudentProfileId { get; set; }
    public int CourseId { get; set; }

    public DateTime EnrolledAt { get; set; }

    public virtual StudentProfile StudentProfile { get; set; } = default!;
    public virtual Course Course { get; set; } = default!;
}

