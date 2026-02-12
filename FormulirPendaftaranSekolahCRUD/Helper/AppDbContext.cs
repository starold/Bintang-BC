using Microsoft.EntityFrameworkCore;
using FormulirPendaftaranSekolahCRUD.Model;

namespace FormulirPendaftaranSekolahCRUD.Helper
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Kelas> Kelass { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Student tidak punya Id property, jadi kita pakai shadow property
            modelBuilder.Entity<Student>()
                .Property<int>("Id")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Student>()
                .HasKey("Id");

            // Relationship: Student -> Kelas (Many-to-One)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Kelas)
                .WithMany(k => k.Students)
                .HasForeignKey(s => s.KelasId);

            // Seed data Kelas
            modelBuilder.Entity<Kelas>().HasData(
                new Kelas { Id = 1, NamaKelas = "IPA", Deskripsi = "Ilmu Pengetahuan Alam" },
                new Kelas { Id = 2, NamaKelas = "IPS", Deskripsi = "Ilmu Pengetahuan Sosial" }
            );
        }
    }
}
