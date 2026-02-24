using System.Data;
using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EnterpriseCRUD.Infrastructure.Services;

/// <summary>
/// Implementation of ISchemaDiscoveryService for SQLite.
/// </summary>
public class SqliteSchemaDiscoveryService : ISchemaDiscoveryService
{
    private readonly string _connectionString;

    public SqliteSchemaDiscoveryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                           ?? "Data Source=EnterpriseCRUD.db";
    }

    public async Task CreateTableAsync(EntityMetadata metadata)
    {
        var columns = new List<string>();
        var foreignKeys = new List<string>();

        // Ensure Id is present
        if (!metadata.Fields.Any(f => f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
        {
            columns.Add("Id TEXT PRIMARY KEY");
        }

        foreach (var field in metadata.Fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            {
                columns.Add("Id TEXT PRIMARY KEY");
                continue;
            }

            var type = MapToSqliteType(field.FieldType);
            var definition = $"{field.Name} {type}";
            
            if (field.IsRequired) definition += " NOT NULL";
            
            columns.Add(definition);

            if (field.FieldType == "reference" && !string.IsNullOrEmpty(field.ReferenceEntity))
            {
                foreignKeys.Add($"FOREIGN KEY ({field.Name}) REFERENCES {field.ReferenceEntity}(Id)");
            }
        }

        var columnSql = string.Join(", ", columns);
        var fkSql = foreignKeys.Count > 0 ? ", " + string.Join(", ", foreignKeys) : "";
        var createSql = $"CREATE TABLE {metadata.Name} ({columnSql}{fkSql});";

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = createSql;
        await command.ExecuteNonQueryAsync();
    }

    private string MapToSqliteType(string fieldType)
    {
        return fieldType.ToLower() switch
        {
            "int" => "INTEGER",
            "decimal" => "REAL",
            "bool" => "INTEGER",
            "date" or "datetime" or "string" or "guid" or "reference" => "TEXT",
            _ => "TEXT"
        };
    }

    public async Task<IEnumerable<EntityMetadata>> DiscoverEntitiesAsync()
    {
        var entities = new List<EntityMetadata>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // 1. Get all tables (excluding system tables)
        var tables = await GetTablesAsync(connection);

        foreach (var tableName in tables)
        {
            var entity = new EntityMetadata
            {
                Name = tableName.ToLower(),
                DisplayName = tableName,
                DisplayNameSingular = tableName.EndsWith("s") ? tableName[..^1] : tableName,
                EntityTypeName = tableName,
                Icon = "list"
            };

            // 2. Get columns for this table
            var columns = await GetColumnsAsync(connection, tableName);
            foreach (var col in columns)
            {
                var field = new FieldMetadata
                {
                    Name = col.ColumnName,
                    Label = col.ColumnName,
                    FieldType = MapDataType(col.DataType),
                    IsRequired = !col.IsNullable,
                    ShowInList = true,
                    ShowInShow = true,
                    ShowInCreate = !col.IsPrimaryKey, // Hide PK in create if it's auto-inc or guid
                    ShowInEdit = !col.IsPrimaryKey,
                    IsSortable = true,
                    IsFilterable = true,
                    IsSearchable = col.DataType.Contains("TEXT", StringComparison.OrdinalIgnoreCase)
                };

                if (!string.IsNullOrEmpty(col.ForeignKeyTable))
                {
                    field.FieldType = "reference";
                    field.ReferenceEntity = col.ForeignKeyTable.ToLower();
                    field.ReferenceDisplayField = "Name"; // Default display field hint
                }

                entity.Fields.Add(field);
            }

            entities.Add(entity);
        }

        return entities;
    }

    private async Task<List<string>> GetTablesAsync(SqliteConnection connection)
    {
        var tables = new List<string>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '__EFMigrationsHistory'";
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        return tables;
    }

    private async Task<List<ColumnDiscoveryInfo>> GetColumnsAsync(SqliteConnection connection, string tableName)
    {
        var columns = new List<ColumnDiscoveryInfo>();
        
        // Use PRAGMA table_info to get basic column info
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName})";
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                columns.Add(new ColumnDiscoveryInfo
                {
                    TableName = tableName,
                    ColumnName = reader.GetString(1),
                    DataType = reader.GetString(2),
                    IsNullable = reader.GetInt32(3) == 0,
                    IsPrimaryKey = reader.GetInt32(5) > 0
                });
            }
        }

        // Use PRAGMA foreign_key_list to get relationships
        command.CommandText = $"PRAGMA foreign_key_list({tableName})";
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var fromCol = reader.GetString(3);
                var toTable = reader.GetString(2);
                var toCol = reader.GetString(4);

                var column = columns.FirstOrDefault(c => c.ColumnName == fromCol);
                if (column != null)
                {
                    column.ForeignKeyTable = toTable;
                    column.ForeignKeyColumn = toCol;
                }
            }
        }

        return columns;
    }

    private string MapDataType(string sqliteType)
    {
        sqliteType = sqliteType.ToUpper();
        if (sqliteType.Contains("INT")) return "int";
        if (sqliteType.Contains("TEXT")) return "string";
        if (sqliteType.Contains("DATE")) return "date";
        if (sqliteType.Contains("DATETIME")) return "datetime";
        if (sqliteType.Contains("DECIMAL") || sqliteType.Contains("NUMERIC") || sqliteType.Contains("REAL")) return "decimal";
        if (sqliteType.Contains("BLOB")) return "string"; // Or handle binary
        if (sqliteType.Contains("BOOLEAN") || sqliteType.Contains("BIT")) return "bool";
        if (sqliteType.Contains("GUID") || sqliteType.Contains("UUID")) return "guid";
        
        return "string";
    }
}
