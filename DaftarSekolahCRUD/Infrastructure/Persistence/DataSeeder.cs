using DaftarSekolahCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DaftarSekolahCRUD.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.ClassRooms.Any())
            {
                var classRooms = new List<ClassRoom>
                {
                    new ClassRoom { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Class 10A" },
                    new ClassRoom { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Class 10B" },
                    new ClassRoom { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Class 11A" }
                };
                
                await context.ClassRooms.AddRangeAsync(classRooms);
                await context.SaveChangesAsync();
            }

            if (!context.Extracurriculars.Any())
            {
                var extracuriculars = new List<Extracurricular>
                {
                    new Extracurricular { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Basketball" },
                    new Extracurricular { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Music" },
                    new Extracurricular { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Programming" }
                };

                await context.Extracurriculars.AddRangeAsync(extracuriculars);
                await context.SaveChangesAsync();
            }
        }
    }
}
