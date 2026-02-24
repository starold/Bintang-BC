using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Domain.Enums;
using EnterpriseCRUD.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCRUD.Infrastructure.Data.Seed;

/// <summary>
/// Seeds the database with roles, users, and sample school data.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // ── Seed Roles ──
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ── Seed Admin User ──
        if (await userManager.FindByEmailAsync("admin@school.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@school.com",
                Email = "admin@school.com",
                FullName = "System Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Seed Regular User ──
        if (await userManager.FindByEmailAsync("user@school.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "user@school.com",
                Email = "user@school.com",
                FullName = "Regular User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
        }

        // ── Seed Sample Data (only if empty) ──
        if (await context.Teachers.AnyAsync()) return;

        // Teachers
        var teacher1 = new Teacher { Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith", Email = "john.smith@school.com", Specialization = "Mathematics" };
        var teacher2 = new Teacher { Id = Guid.NewGuid(), FirstName = "Sarah", LastName = "Johnson", Email = "sarah.j@school.com", Specialization = "Science" };
        var teacher3 = new Teacher { Id = Guid.NewGuid(), FirstName = "David", LastName = "Williams", Email = "david.w@school.com", Specialization = "English" };
        context.Teachers.AddRange(teacher1, teacher2, teacher3);

        // Subjects
        var math = new Subject { Id = Guid.NewGuid(), Name = "Mathematics", Code = "MATH101", Credits = 4 };
        var physics = new Subject { Id = Guid.NewGuid(), Name = "Physics", Code = "PHYS101", Credits = 3 };
        var english = new Subject { Id = Guid.NewGuid(), Name = "English Literature", Code = "ENG101", Credits = 3 };
        var chemistry = new Subject { Id = Guid.NewGuid(), Name = "Chemistry", Code = "CHEM101", Credits = 3 };
        context.Subjects.AddRange(math, physics, english, chemistry);

        // Teacher-Subject (Many-to-Many)
        context.TeacherSubjects.AddRange(
            new TeacherSubject { TeacherId = teacher1.Id, SubjectId = math.Id },
            new TeacherSubject { TeacherId = teacher2.Id, SubjectId = physics.Id },
            new TeacherSubject { TeacherId = teacher2.Id, SubjectId = chemistry.Id },
            new TeacherSubject { TeacherId = teacher3.Id, SubjectId = english.Id }
        );

        // ClassRooms
        var room1 = new ClassRoom { Id = Guid.NewGuid(), Name = "Room 101", Capacity = 30, TeacherId = teacher1.Id };
        var room2 = new ClassRoom { Id = Guid.NewGuid(), Name = "Room 202", Capacity = 25, TeacherId = teacher2.Id };
        var room3 = new ClassRoom { Id = Guid.NewGuid(), Name = "Room 303", Capacity = 35, TeacherId = teacher3.Id };
        context.ClassRooms.AddRange(room1, room2, room3);

        // Students
        var student1 = new Student { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Anderson", Email = "alice@student.com", DateOfBirth = new DateTime(2005, 3, 15), Gender = Gender.Female };
        var student2 = new Student { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Baker", Email = "bob@student.com", DateOfBirth = new DateTime(2004, 7, 22), Gender = Gender.Male };
        var student3 = new Student { Id = Guid.NewGuid(), FirstName = "Charlie", LastName = "Chen", Email = "charlie@student.com", DateOfBirth = new DateTime(2005, 1, 10), Gender = Gender.Male };
        var student4 = new Student { Id = Guid.NewGuid(), FirstName = "Diana", LastName = "Davis", Email = "diana@student.com", DateOfBirth = new DateTime(2004, 11, 5), Gender = Gender.Female };
        var student5 = new Student { Id = Guid.NewGuid(), FirstName = "Ethan", LastName = "Evans", Email = "ethan@student.com", DateOfBirth = new DateTime(2005, 6, 18), Gender = Gender.Male };
        context.Students.AddRange(student1, student2, student3, student4, student5);

        // Enrollments
        context.Enrollments.AddRange(
            new Enrollment { StudentId = student1.Id, SubjectId = math.Id, ClassRoomId = room1.Id, EnrollmentDate = DateTime.UtcNow, Grade = "A" },
            new Enrollment { StudentId = student1.Id, SubjectId = physics.Id, ClassRoomId = room2.Id, EnrollmentDate = DateTime.UtcNow, Grade = "B+" },
            new Enrollment { StudentId = student2.Id, SubjectId = math.Id, ClassRoomId = room1.Id, EnrollmentDate = DateTime.UtcNow, Grade = "B" },
            new Enrollment { StudentId = student2.Id, SubjectId = english.Id, ClassRoomId = room3.Id, EnrollmentDate = DateTime.UtcNow, Grade = "A-" },
            new Enrollment { StudentId = student3.Id, SubjectId = physics.Id, ClassRoomId = room2.Id, EnrollmentDate = DateTime.UtcNow, Grade = "A" },
            new Enrollment { StudentId = student3.Id, SubjectId = chemistry.Id, ClassRoomId = room2.Id, EnrollmentDate = DateTime.UtcNow },
            new Enrollment { StudentId = student4.Id, SubjectId = english.Id, ClassRoomId = room3.Id, EnrollmentDate = DateTime.UtcNow, Grade = "A+" },
            new Enrollment { StudentId = student5.Id, SubjectId = math.Id, ClassRoomId = room1.Id, EnrollmentDate = DateTime.UtcNow, Grade = "B-" }
        );

        await context.SaveChangesAsync();
    }
}
