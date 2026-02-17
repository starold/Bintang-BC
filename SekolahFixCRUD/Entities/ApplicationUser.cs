using Microsoft.AspNetCore.Identity;

namespace SekolahFixCRUD.Entities;


public class ApplicationUser : IdentityUser
{
    public virtual StudentProfile? StudentProfile { get; set; }
}

