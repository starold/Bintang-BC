using Microsoft.AspNetCore.Identity;
using SekolahFixCRUD.Data;
using SekolahFixCRUD.Entities;

namespace SekolahFixCRUD.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Roles
        string[] roles = { "Admin", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Admin User
        var adminEmail = "admin@sekolah.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // 3. Teachers
        if (!context.Teachers.Any())
        {
            var teachers = new List<Teacher>
            {
                new Teacher { FullName = "Prof. Budi Santoso", Email = "budi@sekolah.com", Department = "Computer Science" },
                new Teacher { FullName = "Dr. Siti Aminah", Email = "siti@sekolah.com", Department = "Mathematics" }
            };
            context.Teachers.AddRange(teachers);
            await context.SaveChangesAsync();
        }

        // 4. Courses
        if (!context.Courses.Any())
        {
            var teacherId = context.Teachers.First().Id;
            var courses = new List<Course>
            {
                new Course { Name = "Web Development", Code = "CS101", Credits = 3, TeacherId = teacherId, Description = "Learn the basics of web dev" },
                new Course { Name = "Data Structures", Code = "CS201", Credits = 4, TeacherId = teacherId, Description = "Advanced algorithm analysis" }
            };
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }
        
        // 5. Sample Students for Testing
        
        // 5a. Approved Student
        await CreateStudentWithProfile(context, userManager, 
            "student@sekolah.com", "Student123!", "Approved Student", 
            "Universitas Gadjah Mada, Yogyakarta", "Approved", true);

        // 5b. Pending Student
        await CreateStudentWithProfile(context, userManager, 
            "pending@sekolah.com", "Student123!", "Pending Student", 
            "Jl. Malioboro, Yogyakarta", "Pending", false);

        // 5c. Rejected Student
        await CreateStudentWithProfile(context, userManager, 
            "rejected@sekolah.com", "Student123!", "Rejected Student", 
            "Jl. Kaliurang, Yogyakarta", "Rejected", false);
    }

    private static async Task CreateStudentWithProfile(
        AppDbContext context, 
        UserManager<ApplicationUser> userManager, 
        string email, 
        string password, 
        string fullName, 
        string address, 
        string status, 
        bool isApproved)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, "Student");

            var profile = new StudentProfile
            {
                UserId = user.Id,
                FullName = fullName,
                Address = address,
                DateOfBirth = new DateTime(2005, 5, 20),
                IsApproved = isApproved,
                ApprovalStatus = status
            };
            context.StudentProfiles.Add(profile);
            await context.SaveChangesAsync();
        }
    }
}
