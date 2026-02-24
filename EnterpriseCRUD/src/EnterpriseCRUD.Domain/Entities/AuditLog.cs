using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Stores change history for auditing purposes.
/// </summary>
public class AuditLog : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Create, Update, Delete

    public string? OldValues { get; set; } // JSON string

    public string? NewValues { get; set; } // JSON string

    [MaxLength(100)]
    public string? ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
