using EnterpriseCRUD.Domain.Metadata;

namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Service to investigate database schema and extract metadata.
/// </summary>
public interface ISchemaDiscoveryService
{
    Task<IEnumerable<EntityMetadata>> DiscoverEntitiesAsync();
    Task CreateTableAsync(EntityMetadata metadata);
}

/// <summary>
/// Result of column discovery.
/// </summary>
public class ColumnDiscoveryInfo
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string? ForeignKeyTable { get; set; }
    public string? ForeignKeyColumn { get; set; }
}
