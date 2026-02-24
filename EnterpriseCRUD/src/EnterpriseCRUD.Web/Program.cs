using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Application.Services;
using EnterpriseCRUD.Infrastructure.Data;
using EnterpriseCRUD.Infrastructure.Data.Seed;
using EnterpriseCRUD.Infrastructure.Identity;
using EnterpriseCRUD.Infrastructure.Repositories;
using EnterpriseCRUD.Infrastructure.Services;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.Domain.Entities; // Added this using directive
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database (SQLite) ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── ASP.NET Identity ──
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Dependency Injection ──
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, EnterpriseCRUD.Infrastructure.UnitOfWork.UnitOfWork>();
builder.Services.AddScoped<ICrudService, GenericCrudService>();
builder.Services.AddScoped<IMetadataStore, DbMetadataStore>();

// ── Controllers & Views ──
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Seed Database ──
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var metadataStore = services.GetRequiredService<IMetadataStore>();
        
        await context.Database.MigrateAsync();
        await DataSeeder.SeedAsync(context, userManager, roleManager);
        await metadataStore.MergeAsync(EntityMetadataRegistry.GetAllMetadata());
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ── Dynamic CRUD Routes ──
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{entityName}/{action=Index}/{id?}",
    defaults: new { controller = "Crud" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
