using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.API.Controllers;

/// <summary>
/// Serves entity metadata to the frontend.
/// The frontend reads this to dynamically generate CRUD UIs.
/// </summary>
[ApiController]
[Route("api/v1/metadata")]
public class MetadataController : ControllerBase
{
    private readonly IMetadataStore _metadataStore;

    public MetadataController(IMetadataStore metadataStore)
    {
        _metadataStore = metadataStore;
    }

    /// <summary>
    /// GET /api/v1/metadata
    /// Returns metadata for all registered entities.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntityMetadata>>> GetAll()
    {
        var all = await _metadataStore.GetAllAsync();
        return Ok(all.Values);
    }

    /// <summary>
    /// GET /api/v1/metadata/{entity}
    /// Returns metadata for a specific entity.
    /// </summary>
    [HttpGet("{entity}")]
    public async Task<ActionResult<EntityMetadata>> Get(string entity)
    {
        var metadata = await _metadataStore.GetAsync(entity);
        if (metadata == null) return NotFound();
        return Ok(metadata);
    }
}
