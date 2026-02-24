using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRUD.Domain.Entities.Metadata;

/// <summary>
/// Database-backed metadata for a dynamic module.
/// </summary>
public class ModuleDefinition : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayNameSingular { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EntityTypeName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Icon { get; set; } = "list";

    [MaxLength(100)]
    public string? Group { get; set; }

    [MaxLength(100)]
    public string DefaultSortField { get; set; } = "CreatedAt";

    [MaxLength(10)]
    public string DefaultSortOrder { get; set; } = "desc";

    public int DefaultPageSize { get; set; } = 25;

    public bool IsActive { get; set; } = true;

    public bool IsVisible { get; set; } = true;

    // Navigation
    public virtual ICollection<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();
    public virtual ICollection<ModulePermission> Permissions { get; set; } = new List<ModulePermission>();
}
