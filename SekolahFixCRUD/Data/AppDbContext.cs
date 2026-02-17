using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SekolahFixCRUD.Entities;

namespace SekolahFixCRUD.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<StudentProfile> StudentProfiles { get; set; } = default!;
    public DbSet<Teacher> Teachers { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<StudentCourse> StudentCourses { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Fluent API Configuration ---

        // One-to-One: ApplicationUser <-> StudentProfile
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.StudentProfile)
            .WithOne(p => p.User)
            .HasForeignKey<StudentProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: Teacher -> Course
        builder.Entity<Teacher>()
            .HasMany(t => t.Courses)
            .WithOne(c => c.Teacher)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many: StudentProfile <-> Course via StudentCourse
        builder.Entity<StudentCourse>()
            .HasKey(sc => new { sc.StudentProfileId, sc.CourseId });

        builder.Entity<StudentCourse>()
            .HasOne(sc => sc.StudentProfile)
            .WithMany(p => p.StudentCourses)
            .HasForeignKey(sc => sc.StudentProfileId);

        builder.Entity<StudentCourse>()
            .HasOne(sc => sc.Course)
            .WithMany(c => c.StudentCourses)
            .HasForeignKey(sc => sc.CourseId);

        // --- Global Query Filters for Soft Delete ---
        builder.Entity<StudentProfile>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Teacher>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Course>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<StudentCourse>().HasQueryFilter(x => !x.IsDeleted);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.IsDeleted = false;
            }
            else
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
