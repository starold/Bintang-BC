using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Application.Common.Models;
using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Domain.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnterpriseCRUD.Application.Services;

/// <summary>
/// Dynamic CRUD service that handles any registered entity using metadata + reflection.
/// This is the core engine that eliminates repetitive CRUD code.
/// </summary>
public class GenericCrudService : ICrudService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMetadataStore _metadataStore;
    private readonly ILogger<GenericCrudService> _logger;

    public GenericCrudService(
        IServiceProvider serviceProvider,
        IUnitOfWork unitOfWork,
        IMetadataStore metadataStore,
        ILogger<GenericCrudService> logger)
    {
        _serviceProvider = serviceProvider;
        _unitOfWork = unitOfWork;
        _metadataStore = metadataStore;
        _logger = logger;
    }

    public async Task<PagedResult> GetPagedAsync(string entityName, int page, int pageSize,
        string? sortField, string? sortOrder, string? search,
        Dictionary<string, string>? filters)
    {
        var metadata = await GetMetadataOrThrowAsync(entityName);
        var entityType = GetEntityTypeOrThrow(metadata);

        // Use reflection to call the generic GetPagedInternal<T> method
        var method = GetType()
            .GetMethod(nameof(GetPagedInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        var result = await (Task<PagedResult>)method.Invoke(this,
            new object?[] { metadata, page, pageSize, sortField, sortOrder, search, filters })!;

        return result;
    }

    public async Task<object?> GetByIdAsync(string entityName, Guid id)
    {
        var metadata = await GetMetadataOrThrowAsync(entityName);
        var entityType = GetEntityTypeOrThrow(metadata);

        var method = GetType()
            .GetMethod(nameof(GetByIdInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        return await (Task<object?>)method.Invoke(this, new object[] { metadata, id })!;
    }

    public async Task<object> CreateAsync(string entityName, Dictionary<string, object?> data)
    {
        var metadata = await GetMetadataOrThrowAsync(entityName);
        var entityType = GetEntityTypeOrThrow(metadata);

        var method = GetType()
            .GetMethod(nameof(CreateInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        return await (Task<object>)method.Invoke(this, new object[] { metadata, data })!;
    }

    public async Task<object?> UpdateAsync(string entityName, Guid id, Dictionary<string, object?> data)
    {
        var metadata = await GetMetadataOrThrowAsync(entityName);
        var entityType = GetEntityTypeOrThrow(metadata);

        var method = GetType()
            .GetMethod(nameof(UpdateInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        return await (Task<object?>)method.Invoke(this, new object[] { metadata, id, data })!;
    }

    public async Task<bool> DeleteAsync(string entityName, Guid id)
    {
        var metadata = await GetMetadataOrThrowAsync(entityName);
        var entityType = GetEntityTypeOrThrow(metadata);

        var method = GetType()
            .GetMethod(nameof(DeleteInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        return await (Task<bool>)method.Invoke(this, new object[] { id })!;
    }

    // ── Internal generic implementations ─────────────────────────────

    private async Task<PagedResult> GetPagedInternal<T>(
        EntityMetadata metadata, int page, int pageSize,
        string? sortField, string? sortOrder, string? search,
        Dictionary<string, string>? filters) where T : BaseEntity
    {
        var repo = GetRepository<T>();
        var includes = metadata.Includes?.ToArray();

        // Build filter expression from search + filters
        Expression<Func<T, bool>>? filterExpr = null;

        if (!string.IsNullOrEmpty(search))
        {
            filterExpr = BuildSearchExpression<T>(metadata, search);
        }

        if (filters != null && filters.Count > 0)
        {
            var filterExpression = BuildFilterExpression<T>(metadata, filters);
            if (filterExpr != null && filterExpression != null)
            {
                // Combine search and filter with AND
                var param = Expression.Parameter(typeof(T), "x");
                var combined = Expression.AndAlso(
                    Expression.Invoke(filterExpr, param),
                    Expression.Invoke(filterExpression, param));
                filterExpr = Expression.Lambda<Func<T, bool>>(combined, param);
            }
            else if (filterExpression != null)
            {
                filterExpr = filterExpression;
            }
        }

        sortField ??= metadata.DefaultSortField;
        sortOrder ??= metadata.DefaultSortOrder;

        var (items, totalCount) = await repo.GetPagedAsync(
            page, pageSize, filterExpr, sortField, sortOrder, includes);

        // Convert entities to dictionaries for JSON output
        var data = items.Select(e => EntityToDictionary(e, metadata)).Cast<object>().ToList();

        return new PagedResult
        {
            Data = data,
            Total = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
        };
    }

    private async Task<object?> GetByIdInternal<T>(EntityMetadata metadata, Guid id) where T : BaseEntity
    {
        var repo = GetRepository<T>();
        var includes = metadata.Includes?.ToArray() ?? Array.Empty<string>();
        var entity = await repo.GetByIdAsync(id, includes);

        if (entity == null) return null;

        return EntityToDictionary(entity, metadata);
    }

    private async Task<object> CreateInternal<T>(EntityMetadata metadata, Dictionary<string, object?> data) where T : BaseEntity
    {
        var repo = GetRepository<T>();
        var entity = Activator.CreateInstance<T>();

        MapDataToEntity(entity, data, metadata);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        var created = await repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created {EntityType} with Id {Id}", typeof(T).Name, created.Id);

        return EntityToDictionary(created, metadata);
    }

    private async Task<object?> UpdateInternal<T>(EntityMetadata metadata, Guid id, Dictionary<string, object?> data) where T : BaseEntity
    {
        var repo = GetRepository<T>();
        var entity = await repo.GetByIdAsync(id);

        if (entity == null) return null;

        MapDataToEntity(entity, data, metadata);
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Updated {EntityType} with Id {Id}", typeof(T).Name, id);

        return EntityToDictionary(entity, metadata);
    }

    private async Task<bool> DeleteInternal<T>(Guid id) where T : BaseEntity
    {
        var repo = GetRepository<T>();
        var entity = await repo.GetByIdAsync(id);

        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deleted {EntityType} with Id {Id}", typeof(T).Name, id);

        return true;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private IGenericRepository<T> GetRepository<T>() where T : BaseEntity
    {
        return _serviceProvider.GetRequiredService<IGenericRepository<T>>();
    }

    private async Task<EntityMetadata> GetMetadataOrThrowAsync(string entityName)
    {
        return await _metadataStore.GetAsync(entityName)
            ?? throw new KeyNotFoundException($"Entity '{entityName}' is not registered in metadata store.");
    }

    private static Type GetEntityTypeOrThrow(EntityMetadata metadata)
    {
        // Resolve type from assembly by EntityTypeName
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => (t.Name == metadata.EntityTypeName || t.FullName == metadata.EntityTypeName) 
                                && typeof(BaseEntity).IsAssignableFrom(t));

        return type ?? throw new KeyNotFoundException($"Entity type '{metadata.EntityTypeName}' for '{metadata.Name}' could not be resolved.");
    }

    /// <summary>
    /// Converts an entity to a dictionary using metadata field definitions.
    /// Includes related entity display fields for reference types.
    /// </summary>
    private static Dictionary<string, object?> EntityToDictionary<T>(T entity, EntityMetadata metadata) where T : BaseEntity
    {
        var dict = new Dictionary<string, object?>();
        var type = entity.GetType();

        foreach (var field in metadata.Fields)
        {
            var prop = type.GetProperty(field.Name);
            if (prop != null)
            {
                var value = prop.GetValue(entity);
                dict[ToCamelCase(field.Name)] = value;
            }

            // For reference fields, also include the display value from the navigation property
            if (field.FieldType == "reference" && field.ReferenceDisplayField != null)
            {
                // Navigate: e.g., for "TeacherId" → navigation property "Teacher"
                var navPropName = field.Name.Replace("Id", "");
                var navProp = type.GetProperty(navPropName);
                if (navProp != null)
                {
                    var navValue = navProp.GetValue(entity);
                    if (navValue != null)
                    {
                        var displayProp = navValue.GetType().GetProperty(field.ReferenceDisplayField);
                        var displayValue = displayProp?.GetValue(navValue);
                        dict[ToCamelCase(navPropName) + "Display"] = displayValue;
                    }
                }
            }
        }

        return dict;
    }

    /// <summary>Maps form data dictionary values to entity properties.</summary>
    private static void MapDataToEntity<T>(T entity, Dictionary<string, object?> data, EntityMetadata metadata) where T : BaseEntity
    {
        var type = entity.GetType();

        foreach (var field in metadata.Fields)
        {
            if (!field.ShowInCreate && !field.ShowInEdit) continue;

            // Try camelCase then PascalCase key lookup
            var camelKey = ToCamelCase(field.Name);
            object? value = null;
            bool hasKey = data.TryGetValue(camelKey, out value) || data.TryGetValue(field.Name, out value);

            if (!hasKey) continue;

            var prop = type.GetProperty(field.Name);
            if (prop == null || !prop.CanWrite) continue;

            try
            {
                var converted = ConvertValue(value, prop.PropertyType);
                prop.SetValue(entity, converted);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to set property '{field.Name}' on '{type.Name}': {ex.Message}", ex);
            }
        }
    }

    /// <summary>Convert a JSON-deserialized value to the target CLR type.</summary>
    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Handle JsonElement from System.Text.Json
        if (value is JsonElement jsonElement)
        {
            return ConvertJsonElement(jsonElement, underlyingType);
        }

        if (underlyingType == typeof(Guid))
            return Guid.Parse(value.ToString()!);

        if (underlyingType == typeof(DateTime))
            return DateTime.Parse(value.ToString()!);

        if (underlyingType.IsEnum)
            return Enum.Parse(underlyingType, value.ToString()!);

        return Convert.ChangeType(value, underlyingType);
    }

    private static object? ConvertJsonElement(JsonElement element, Type targetType)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String when targetType == typeof(Guid) => Guid.Parse(element.GetString()!),
            JsonValueKind.String when targetType == typeof(DateTime) => DateTime.Parse(element.GetString()!),
            JsonValueKind.String when targetType.IsEnum => Enum.Parse(targetType, element.GetString()!),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when targetType == typeof(int) => element.GetInt32(),
            JsonValueKind.Number when targetType == typeof(long) => element.GetInt64(),
            JsonValueKind.Number when targetType == typeof(double) => element.GetDouble(),
            JsonValueKind.Number when targetType == typeof(decimal) => element.GetDecimal(),
            JsonValueKind.True or JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }

    /// <summary>Build an expression that searches across all searchable fields.</summary>
    private static Expression<Func<T, bool>>? BuildSearchExpression<T>(EntityMetadata metadata, string search) where T : BaseEntity
    {
        var searchableFields = metadata.Fields.Where(f => f.IsSearchable).ToList();
        if (!searchableFields.Any()) return null;

        var param = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;
        var searchLower = search.ToLower();
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;

        foreach (var field in searchableFields)
        {
            var prop = typeof(T).GetProperty(field.Name);
            if (prop == null || prop.PropertyType != typeof(string)) continue;

            // x.PropName != null && x.PropName.ToLower().Contains(search)
            var propAccess = Expression.Property(param, prop);
            var notNull = Expression.NotEqual(propAccess, Expression.Constant(null, typeof(string)));
            var toLower = Expression.Call(propAccess, toLowerMethod);
            var contains = Expression.Call(toLower, containsMethod, Expression.Constant(searchLower));
            var combined2 = Expression.AndAlso(notNull, contains);

            combined = combined == null ? combined2 : Expression.OrElse(combined, combined2);
        }

        return combined != null ? Expression.Lambda<Func<T, bool>>(combined, param) : null;
    }

    /// <summary>Build filter expressions from key-value filter pairs.</summary>
    private static Expression<Func<T, bool>>? BuildFilterExpression<T>(
        EntityMetadata metadata, Dictionary<string, string> filters) where T : BaseEntity
    {
        var param = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;

        foreach (var (key, value) in filters)
        {
            // Find the matching field in metadata (case-insensitive)
            var field = metadata.Fields.FirstOrDefault(f =>
                f.IsFilterable && f.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (field == null) continue;

            var prop = typeof(T).GetProperty(field.Name);
            if (prop == null) continue;

            var propAccess = Expression.Property(param, prop);
            var targetValue = ConvertValue(value, prop.PropertyType);
            var constant = Expression.Constant(targetValue, prop.PropertyType);
            var equals = Expression.Equal(propAccess, constant);

            combined = combined == null ? equals : Expression.AndAlso(combined, equals);
        }

        return combined != null ? Expression.Lambda<Func<T, bool>>(combined, param) : null;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}
