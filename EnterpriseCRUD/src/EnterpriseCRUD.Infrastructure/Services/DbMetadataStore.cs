using System.Text.Json;
using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Entities.Metadata;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCRUD.Infrastructure.Services;

/// <summary>
/// Database implementation of IMetadataStore.
/// Maps the database entities back to the domain metadata POCOs used by GenericCrudService.
/// </summary>
public class DbMetadataStore : IMetadataStore
{
    private readonly AppDbContext _context;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public DbMetadataStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, EntityMetadata>> GetAllAsync()
    {
        var modules = await _context.ModuleDefinitions
            .Include(m => m.Fields)
            .Where(m => m.IsActive)
            .ToListAsync();

        return modules.ToDictionary(m => m.Name, MapToDomain);
    }

    public async Task<EntityMetadata?> GetAsync(string entityName)
    {
        var module = await _context.ModuleDefinitions
            .Include(m => m.Fields)
            .FirstOrDefaultAsync(m => m.Name == entityName && m.IsActive);

        return module != null ? MapToDomain(module) : null;
    }

    public async Task SaveAsync(string entityName, EntityMetadata metadata)
    {
        var module = await _context.ModuleDefinitions
            .Include(m => m.Fields)
            .FirstOrDefaultAsync(m => m.Name == entityName);

        if (module == null)
        {
            module = new ModuleDefinition { Name = entityName };
            _context.ModuleDefinitions.Add(module);
        }

        UpdateModuleFromMetadata(module, metadata);
        await _context.SaveChangesAsync();
    }

    public async Task InitializeAsync(IEnumerable<EntityMetadata> seedMetadata)
    {
        if (await _context.ModuleDefinitions.AnyAsync()) return;

        foreach (var meta in seedMetadata)
        {
            var module = new ModuleDefinition { Name = meta.Name };
            UpdateModuleFromMetadata(module, meta);
            _context.ModuleDefinitions.Add(module);
        }

        await _context.SaveChangesAsync();
    }

    public async Task MergeAsync(IEnumerable<EntityMetadata> metadata)
    {
        foreach (var meta in metadata)
        {
            var exists = await _context.ModuleDefinitions.AnyAsync(m => m.Name == meta.Name);
            if (!exists)
            {
                var module = new ModuleDefinition { Name = meta.Name };
                UpdateModuleFromMetadata(module, meta);
                _context.ModuleDefinitions.Add(module);
            }
        }

        await _context.SaveChangesAsync();
    }

    private static EntityMetadata MapToDomain(ModuleDefinition module)
    {
        return new EntityMetadata
        {
            Name = module.Name,
            DisplayName = module.DisplayName,
            DisplayNameSingular = module.DisplayNameSingular,
            EntityTypeName = module.EntityTypeName,
            Icon = module.Icon,
            Group = module.Group,
            DefaultSortField = module.DefaultSortField,
            DefaultSortOrder = module.DefaultSortOrder,
            DefaultPageSize = module.DefaultPageSize,
            IsVisible = module.IsVisible,
            Fields = module.Fields.OrderBy(f => f.DisplayOrder).Select(f => new FieldMetadata
            {
                Name = f.Name,
                Label = f.Label,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                IsSearchable = f.IsSearchable,
                IsFilterable = f.IsFilterable,
                IsSortable = f.IsSortable,
                ShowInList = f.ShowInList,
                ShowInShow = f.ShowInShow,
                ShowInCreate = f.ShowInCreate,
                ShowInEdit = f.ShowInEdit,
                ReferenceEntity = f.ReferenceEntity,
                ReferenceDisplayField = f.ReferenceDisplayField,
                Choices = string.IsNullOrEmpty(f.ChoicesJson) 
                    ? null 
                    : JsonSerializer.Deserialize<List<EnumChoice>>(f.ChoicesJson, _jsonOptions),
                MaxLength = f.MaxLength,
                Min = f.MinValue,
                Max = f.MaxValue,
                DefaultValue = f.DefaultValue,
                Placeholder = f.Placeholder,
                ValidationRegex = f.ValidationRegex,
                Tooltip = f.Tooltip,
                IsReadOnly = f.IsReadOnly
            }).ToList()
        };
    }

    private static void UpdateModuleFromMetadata(ModuleDefinition module, EntityMetadata meta)
    {
        module.DisplayName = meta.DisplayName;
        module.DisplayNameSingular = meta.DisplayNameSingular;
        module.EntityTypeName = meta.EntityTypeName;
        module.Icon = meta.Icon;
        module.Group = meta.Group;
        module.DefaultSortField = meta.DefaultSortField;
        module.DefaultSortOrder = meta.DefaultSortOrder;
        module.DefaultPageSize = meta.DefaultPageSize;
        module.IsVisible = meta.IsVisible;

        // Simple sync for fields (can be improved)
        module.Fields.Clear();
        int order = 0;
        foreach (var f in meta.Fields)
        {
            module.Fields.Add(new FieldDefinition
            {
                Name = f.Name,
                Label = f.Label,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                IsSearchable = f.IsSearchable,
                IsFilterable = f.IsFilterable,
                IsSortable = f.IsSortable,
                ShowInList = f.ShowInList,
                ShowInShow = f.ShowInShow,
                ShowInCreate = f.ShowInCreate,
                ShowInEdit = f.ShowInEdit,
                ReferenceEntity = f.ReferenceEntity,
                ReferenceDisplayField = f.ReferenceDisplayField,
                ChoicesJson = f.Choices != null ? JsonSerializer.Serialize(f.Choices) : null,
                MaxLength = f.MaxLength,
                MinValue = f.Min,
                MaxValue = f.Max,
                DefaultValue = f.DefaultValue,
                Placeholder = f.Placeholder,
                DisplayOrder = order++,
                ValidationRegex = f.ValidationRegex,
                Tooltip = f.Tooltip,
                IsReadOnly = f.IsReadOnly
            });
        }
    }
}
