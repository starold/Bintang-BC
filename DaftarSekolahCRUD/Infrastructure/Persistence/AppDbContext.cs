using DaftarSekolahCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DaftarSekolahCRUD.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students=> Set<Student>();
        
    }
}