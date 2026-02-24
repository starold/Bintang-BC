using EnterpriseCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseCRUD.Infrastructure.Data.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(s => s.Code).IsUnique();
        builder.Property(s => s.Credits).IsRequired();

        builder.HasMany(s => s.TeacherSubjects)
            .WithOne(ts => ts.Subject)
            .HasForeignKey(ts => ts.SubjectId);

        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Subject)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
