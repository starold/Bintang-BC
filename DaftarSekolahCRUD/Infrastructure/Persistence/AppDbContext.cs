using DaftarSekolahCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace DaftarSekolahCRUD.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students=> Set<Student>();
        public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
        public DbSet<ClassRoom> ClassRooms=> Set<ClassRoom>();
        public DbSet<Extracurricular> Extracurriculars=> Set<Extracurricular>();
        public DbSet<StudentExtracurricular> StudentExtracurriculars=> Set<StudentExtracurricular>();
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options){}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentProfile>()
                .HasKey(sp => sp.StudentId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Profile)
                .WithOne(sp => sp.Student)
                .HasForeignKey<StudentProfile>(sp => sp.StudentId);

            modelBuilder.Entity<StudentExtracurricular>()
                .HasKey(se => new { se.StudentId, se.ExtracurricularId });

            modelBuilder.Entity<Student>()
            
                .HasOne(s => s.ClassRoom)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassRoomId);
            
        }
    }
}