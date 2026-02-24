using System.Text.Json;
using EnterpriseCRUD.Domain.Metadata;

namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Handles persistence of entity metadata to a JSON file.
/// </summary>
public interface IMetadataStore
{
    Task<Dictionary<string, EntityMetadata>> GetAllAsync();
    Task<EntityMetadata?> GetAsync(string entityName);
    Task SaveAsync(string entityName, EntityMetadata metadata);
    Task InitializeAsync(IEnumerable<EntityMetadata> seedMetadata);
    Task MergeAsync(IEnumerable<EntityMetadata> metadata);
}

/// <summary>
/// JSON implementation of MetadataStore.
/// </summary>
public class JsonMetadataStore : IMetadataStore
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public JsonMetadataStore(string rootPath)
    {
        _filePath = Path.Combine(rootPath, "metadata.json");
    }

    public async Task<Dictionary<string, EntityMetadata>> GetAllAsync()
    {
        if (!File.Exists(_filePath)) return new Dictionary<string, EntityMetadata>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<Dictionary<string, EntityMetadata>>(json, _options) 
               ?? new Dictionary<string, EntityMetadata>();
    }

    public async Task<EntityMetadata?> GetAsync(string entityName)
    {
        var all = await GetAllAsync();
        return all.TryGetValue(entityName, out var metadata) ? metadata : null;
    }

    public async Task SaveAsync(string entityName, EntityMetadata metadata)
    {
        var all = await GetAllAsync();
        all[entityName] = metadata;
        var json = JsonSerializer.Serialize(all, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task InitializeAsync(IEnumerable<EntityMetadata> seedMetadata)
    {
        if (File.Exists(_filePath)) return;

        var dict = seedMetadata.ToDictionary(m => m.Name);
        var json = JsonSerializer.Serialize(dict, _options);
        
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        await File.WriteAllTextAsync(_filePath, json);
    }
    
    public async Task MergeAsync(IEnumerable<EntityMetadata> metadata)
    {
        var all = await GetAllAsync();
        bool changed = false;
        
        foreach (var meta in metadata)
        {
            if (!all.ContainsKey(meta.Name))
            {
                all[meta.Name] = meta;
                changed = true;
            }
        }
        
        if (changed || !File.Exists(_filePath))
        {
            var json = JsonSerializer.Serialize(all, _options);
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
