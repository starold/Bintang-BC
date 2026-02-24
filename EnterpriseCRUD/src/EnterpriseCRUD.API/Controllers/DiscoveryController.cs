using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.API.Controllers;

/// <summary>
/// Controller to trigger database schema discovery and metadata sync.
/// </summary>
[ApiController]
[Route("api/v1/discovery")]
[Authorize(Roles = "Admin")]
public class DiscoveryController : ControllerBase
{
    private readonly ISchemaDiscoveryService _discoveryService;
    private readonly IMetadataStore _metadataStore;
    private readonly CodeGenerationService _codeGen;

    public DiscoveryController(ISchemaDiscoveryService discoveryService, IMetadataStore metadataStore, CodeGenerationService codeGen)
    {
        _discoveryService = discoveryService;
        _metadataStore = metadataStore;
        _codeGen = codeGen;
    }

    /// <summary>
    /// POST /api/v1/discovery/sync
    /// Scans the database and merges it with existing metadata.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        var discoveredEntities = await _discoveryService.DiscoverEntitiesAsync();
        var existingMetadata = await _metadataStore.GetAllAsync();

        foreach (var discovered in discoveredEntities)
        {
            if (existingMetadata.TryGetValue(discovered.Name, out var existing))
            {
                // Merge discovered schema into existing (preserving UI configuration)
                SyncFields(existing, discovered);
            }
            else
            {
                // New entity found
                existingMetadata[discovered.Name] = discovered;
                
                // Generate code for new entity
                _codeGen.GenerateEntity(discovered);
                _codeGen.GenerateConfiguration(discovered);
            }
        }

        foreach (var meta in existingMetadata.Values)
        {
            await _metadataStore.SaveAsync(meta.Name, meta);
        }

        return Ok(new { message = "Schema sync successful", entities = existingMetadata.Count });
    }

    /// <summary>
    /// POST /api/v1/discovery/entities
    /// Creates a new table in the database and generates code.
    /// </summary>
    [HttpPost("entities")]
    public async Task<IActionResult> CreateEntity([FromBody] EntityMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.Name)) return BadRequest("Entity name is required");

        try
        {
            // 1. Create table in DB
            await _discoveryService.CreateTableAsync(metadata);

            // 2. Add to metadata store
            await _metadataStore.SaveAsync(metadata.Name, metadata);

            // 3. Generate code
            _codeGen.GenerateEntity(metadata);
            _codeGen.GenerateConfiguration(metadata);

            return Ok(new { message = $"Entity '{metadata.Name}' created and code generated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private void SyncFields(EntityMetadata existing, EntityMetadata discovered)
    {
        // Add new fields, update existing field types
        foreach (var discField in discovered.Fields)
        {
            var existingField = existing.Fields.FirstOrDefault(f => f.Name == discField.Name);
            if (existingField == null)
            {
                existing.Fields.Add(discField);
            }
            else
            {
                // Update technical properties, preserve UI properties
                existingField.FieldType = discField.FieldType;
                existingField.IsRequired = discField.IsRequired;
                existingField.ReferenceEntity = discField.ReferenceEntity;
                existingField.ReferenceDisplayField = discField.ReferenceDisplayField;
            }
        }

        // Optional: remove fields that no longer exist in DB? 
        // Better to flag them as "Deleted" or "Missing" instead of removing to avoid data loss in UI config
    }
}
