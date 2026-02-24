using EnterpriseCRUD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCRUD.Web.Filters;

/// <summary>
/// Checks if the current user has permission to access the requested module.
/// </summary>
public class DynamicModuleAuthorizeAttribute : TypeFilterAttribute
{
    public DynamicModuleAuthorizeAttribute() : base(typeof(DynamicModuleAuthorizeFilter))
    {
    }
}

public class DynamicModuleAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly AppDbContext _context;

    public DynamicModuleAuthorizeFilter(AppDbContext context)
    {
        _context = context;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var entityName = context.RouteData.Values["entityName"]?.ToString();
        if (string.IsNullOrEmpty(entityName)) return;

        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new ChallengeResult();
            return;
        }

        // Get module permissions for the user's roles
        var userRoles = user.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var permission = await _context.ModulePermissions
            .Include(p => p.Module)
            .Where(p => p.Module.Name == entityName && userRoles.Contains(p.RoleName))
            .FirstOrDefaultAsync();

        if (permission == null)
        {
            // If no explicit permission is set, check if it's an admin
            if (userRoles.Contains("Admin")) return;
            
            context.Result = new ForbidResult();
            return;
        }

        var action = context.RouteData.Values["action"]?.ToString()?.ToLower();
        bool authorized = action switch
        {
            "index" => permission.CanView,
            "create" => permission.CanCreate,
            "edit" => permission.CanEdit,
            "delete" => permission.CanDelete,
            "export" => permission.CanExport,
            _ => true
        };

        if (!authorized)
        {
            context.Result = new ForbidResult();
        }
    }
}
