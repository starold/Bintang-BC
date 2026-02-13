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

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options){}
    }
}