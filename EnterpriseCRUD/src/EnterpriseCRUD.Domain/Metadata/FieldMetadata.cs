using System.Text.Json.Serialization;

namespace EnterpriseCRUD.Domain.Metadata;

/// <summary>
/// Describes a single field in an entity for dynamic CRUD rendering.
/// </summary>
public class FieldMetadata
{
    /// <summary>Property name in the entity (PascalCase).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable label for UI display.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Field type: string, int, decimal, date, datetime, email, enum, guid, reference, bool.</summary>
    public string FieldType { get; set; } = "string";

    /// <summary>Whether this field is required for create/edit.</summary>
    public bool IsRequired { get; set; }

    /// <summary>Whether this field is searchable (global search).</summary>
    public bool IsSearchable { get; set; }

    /// <summary>Whether this field can be used as a filter.</summary>
    public bool IsFilterable { get; set; }

    /// <summary>Whether this field is sortable.</summary>
    public bool IsSortable { get; set; } = true;

    /// <summary>Whether to show this field in the list/table view.</summary>
    public bool ShowInList { get; set; } = true;

    /// <summary>Whether to show this field in the detail/show view.</summary>
    public bool ShowInShow { get; set; } = true;

    /// <summary>Whether to include this field in the create form.</summary>
    public bool ShowInCreate { get; set; } = true;

    /// <summary>Whether to include this field in the edit form.</summary>
    public bool ShowInEdit { get; set; } = true;

    /// <summary>For 'reference' fields: the target entity name.</summary>
    public string? ReferenceEntity { get; set; }

    /// <summary>For 'reference' fields: which field to display from the referenced entity.</summary>
    public string? ReferenceDisplayField { get; set; }

    /// <summary>For 'enum' fields: the available choices.</summary>
    public List<EnumChoice>? Choices { get; set; }

    /// <summary>Maximum length for string fields (0 = unlimited).</summary>
    public int MaxLength { get; set; }

    /// <summary>Minimum value for numeric fields.</summary>
    public double? Min { get; set; }

    /// <summary>Maximum value for numeric fields.</summary>
    public double? Max { get; set; }

    /// <summary>Default value as a string.</summary>
    public string? DefaultValue { get; set; }

    /// <summary>Placeholder text for form inputs.</summary>
    public string? Placeholder { get; set; }

    // Security & Advanced UI
    public List<string> ViewRoles { get; set; } = new();
    public List<string> EditRoles { get; set; } = new();
    public bool IsReadOnly { get; set; }
    public string? ValidationRegex { get; set; }
    public string? Tooltip { get; set; }
}

public class EnumChoice
{
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
}
