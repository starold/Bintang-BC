using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseCRUD.Domain.Entities.Metadata;

/// <summary>
/// Database-backed metadata for an entity field.
/// </summary>
public class FieldDefinition : BaseEntity
{
    public Guid ModuleDefinitionId { get; set; }
    
    [ForeignKey(nameof(ModuleDefinitionId))]
    public virtual ModuleDefinition Module { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string FieldType { get; set; } = "string";

    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSortable { get; set; } = true;

    public bool ShowInList { get; set; } = true;
    public bool ShowInShow { get; set; } = true;
    public bool ShowInCreate { get; set; } = true;
    public bool ShowInEdit { get; set; } = true;

    [MaxLength(100)]
    public string? ReferenceEntity { get; set; }

    [MaxLength(100)]
    public string? ReferenceDisplayField { get; set; }

    public string? ChoicesJson { get; set; } // Stored as JSON string

    public int MaxLength { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }

    public string? DefaultValue { get; set; }
    
    [MaxLength(200)]
    public string? Placeholder { get; set; }

    public int DisplayOrder { get; set; }

    // Security
    public string? ViewRolesJson { get; set; }
    public string? EditRolesJson { get; set; }
    public bool IsReadOnly { get; set; }

    public string? ValidationRegex { get; set; }
    
    [MaxLength(1000)]
    public string? Tooltip { get; set; }

    // Relationship mapping
    [MaxLength(50)]
    public string? RelationshipType { get; set; } // OneToOne, OneToMany, ManyToMany
}
