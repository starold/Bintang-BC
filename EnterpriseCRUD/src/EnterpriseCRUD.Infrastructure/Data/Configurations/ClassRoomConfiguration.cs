using EnterpriseCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseCRUD.Infrastructure.Data.Configurations;

public class ClassRoomConfiguration : IEntityTypeConfiguration<ClassRoom>
{
    public void Configure(EntityTypeBuilder<ClassRoom> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Capacity).IsRequired();

        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.ClassRoom)
            .HasForeignKey(e => e.ClassRoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
