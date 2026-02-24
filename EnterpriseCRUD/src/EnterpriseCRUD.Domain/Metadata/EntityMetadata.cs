namespace EnterpriseCRUD.Domain.Metadata;

/// <summary>
/// Describes an entire entity for dynamic CRUD rendering.
/// This is the metadata that the frontend reads to auto-generate
/// list pages, create/edit forms, filters, and search.
/// </summary>
public class EntityMetadata
{
    /// <summary>Internal entity name used in API routes (lowercase, e.g. "students").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable display name (e.g. "Students").</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Singular display name (e.g. "Student").</summary>
    public string DisplayNameSingular { get; set; } = string.Empty;

    /// <summary>The CLR type name of the entity (for reflection).</summary>
    public string EntityTypeName { get; set; } = string.Empty;

    /// <summary>Icon name for sidebar (Material Icons name).</summary>
    public string Icon { get; set; } = "list";

    /// <summary>Fields in this entity.</summary>
    public List<FieldMetadata> Fields { get; set; } = new();

    /// <summary>Default sort field.</summary>
    public string DefaultSortField { get; set; } = "CreatedAt";

    /// <summary>Default sort order: "asc" or "desc".</summary>
    public string DefaultSortOrder { get; set; } = "desc";

    /// <summary>Default page size.</summary>
    public int DefaultPageSize { get; set; } = 25;

    /// <summary>Roles allowed to view this entity (empty = all authenticated).</summary>
    public List<string> AllowedRoles { get; set; } = new();

    /// <summary>Navigation properties to include when querying (for EF eager loading).</summary>
    public List<string> Includes { get; set; } = new();

    /// <summary>Resource group for UI sidebar grouping.</summary>
    public string? Group { get; set; }

    /// <summary>Whether to show this module in the sidebar menu.</summary>
    public bool IsActive { get; set; } = true;

    public bool IsVisible { get; set; } = true;
}
