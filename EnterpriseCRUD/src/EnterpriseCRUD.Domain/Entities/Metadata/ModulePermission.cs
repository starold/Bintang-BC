using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseCRUD.Domain.Entities.Metadata;

/// <summary>
/// Defines which roles have what access to a module.
/// </summary>
public class ModulePermission : BaseEntity
{
    public Guid ModuleDefinitionId { get; set; }
    
    [ForeignKey(nameof(ModuleDefinitionId))]
    public virtual ModuleDefinition Module { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;

    public bool CanView { get; set; } = true;
    public bool CanCreate { get; set; } = true;
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
    public bool CanExport { get; set; } = true;
}
