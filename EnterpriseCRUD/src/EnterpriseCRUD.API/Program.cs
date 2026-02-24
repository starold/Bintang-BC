using System.Text;
using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Application.Services;
using EnterpriseCRUD.Infrastructure.Data;
using EnterpriseCRUD.Infrastructure.Data.Seed;
using EnterpriseCRUD.Infrastructure.Identity;
using EnterpriseCRUD.Infrastructure.Repositories;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// ── Serilog Bootstrap ──
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ── Controllers & Swagger ──
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "EnterpriseCRUD API",
            Version = "v1",
            Description = "Dynamic CRUD API with metadata-driven entity management"
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    // ── CORS (allow React Admin frontend) ──
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Range");
        });
    });

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

    // ── JWT Authentication ──
    var jwtKey = builder.Configuration["JWT:Key"]
        ?? "EnterpriseCRUDSuperSecretKey2025ThatIsLongEnough!";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"] ?? "EnterpriseCRUD",
            ValidAudience = builder.Configuration["JWT:Audience"] ?? "EnterpriseCRUD",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    // ── Dependency Injection ──
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IUnitOfWork, EnterpriseCRUD.Infrastructure.UnitOfWork.UnitOfWork>();
    builder.Services.AddScoped<ICrudService, GenericCrudService>();
    builder.Services.AddScoped<IAuthService, EnterpriseCRUD.Infrastructure.Services.AuthService>();
    builder.Services.AddScoped<JwtTokenGenerator>();
    builder.Services.AddScoped<ISchemaDiscoveryService, EnterpriseCRUD.Infrastructure.Services.SqliteSchemaDiscoveryService>();
    builder.Services.AddScoped<IMetadataStore, EnterpriseCRUD.Infrastructure.Services.DbMetadataStore>();
    builder.Services.AddSingleton(new EnterpriseCRUD.Infrastructure.Services.CodeGenerationService(Path.Combine(builder.Environment.ContentRootPath, "..")));

    var app = builder.Build();

    // ── Middleware Pipeline ──
    app.UseCors("AllowFrontend");
    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

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
            
            // 1. Always prioritize Metadata Merge (Critical for UI)
            try 
            {
                var legacyMetadata = EntityMetadataRegistry.GetAllMetadata();
                await metadataStore.MergeAsync(legacyMetadata);
                Log.Information("Metadata merged successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to merge metadata.");
            }

            // 2. Database Seeding (Non-blocking if possible)
            try
            {
                await DataSeeder.SeedAsync(context, userManager, roleManager);
                Log.Information("Database seeded successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while seeding the database.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Initialization scope failed.");
        }
    }

    Log.Information("EnterpriseCRUD API starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
