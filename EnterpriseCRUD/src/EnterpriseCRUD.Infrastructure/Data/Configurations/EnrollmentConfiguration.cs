using EnterpriseCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseCRUD.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EnrollmentDate).IsRequired();
        builder.Property(e => e.Grade).HasMaxLength(5);

        // Composite unique index to prevent duplicate enrollments
        builder.HasIndex(e => new { e.StudentId, e.SubjectId, e.ClassRoomId }).IsUnique();
    }
}
