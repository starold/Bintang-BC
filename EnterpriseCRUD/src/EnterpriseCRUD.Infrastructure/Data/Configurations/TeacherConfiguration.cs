using EnterpriseCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseCRUD.Infrastructure.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.LastName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(t => t.Email).IsUnique();
        builder.Property(t => t.Specialization).IsRequired().HasMaxLength(200);

        builder.HasMany(t => t.ClassRooms)
            .WithOne(c => c.Teacher)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.TeacherSubjects)
            .WithOne(ts => ts.Teacher)
            .HasForeignKey(ts => ts.TeacherId);
    }
}
