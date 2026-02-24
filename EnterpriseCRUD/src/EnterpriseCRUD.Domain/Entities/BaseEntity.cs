namespace EnterpriseCRUD.Domain.Entities;

/// <summary>
/// Abstract base entity with common audit fields.
/// All domain entities must inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public int Version { get; set; }
}
