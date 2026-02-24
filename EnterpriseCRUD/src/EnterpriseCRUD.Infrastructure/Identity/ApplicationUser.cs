using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRUD.Infrastructure.Identity;

/// <summary>
/// Extended Identity user with a FullName property.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
