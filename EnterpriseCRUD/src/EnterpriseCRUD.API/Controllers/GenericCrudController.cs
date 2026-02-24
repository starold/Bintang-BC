using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.API.Controllers;

/// <summary>
/// Single generic controller that handles ALL entity CRUD operations.
/// Routes: /api/v1/{entity}
/// No need to create per-entity controllers.
/// </summary>
[ApiController]
[Route("api/v1/{entity}")]
[Authorize]
public class GenericCrudController : ControllerBase
{
    private readonly ICrudService _crudService;
    private readonly IMetadataStore _metadataStore;
    private readonly ILogger<GenericCrudController> _logger;

    public GenericCrudController(ICrudService crudService, IMetadataStore metadataStore, ILogger<GenericCrudController> logger)
    {
        _crudService = crudService;
        _metadataStore = metadataStore;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/v1/{entity}
    /// List with pagination, sorting, searching, and filtering.
    /// Compatible with React Admin's data provider.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList(
        string entity,
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_perPage")] int perPage = 25,
        [FromQuery(Name = "_sortField")] string? sortField = null,
        [FromQuery(Name = "_sortOrder")] string? sortOrder = null,
        [FromQuery(Name = "_search")] string? search = null)
    {
        // Validate entity exists
        var metadata = await _metadataStore.GetAsync(entity);
        if (metadata == null)
            return NotFound(new { message = $"Entity '{entity}' not found." });

        // Collect filter query parameters (filter[fieldName]=value)
        var filters = new Dictionary<string, string>();
        foreach (var key in Request.Query.Keys)
        {
            if (key.StartsWith("filter[") && key.EndsWith("]"))
            {
                var fieldName = key[7..^1]; // extract between [ and ]
                filters[fieldName] = Request.Query[key].ToString();
            }
        }

        var result = await _crudService.GetPagedAsync(entity, page, perPage, sortField, sortOrder, search, filters);

        // React Admin expects Content-Range header for pagination
        Response.Headers["Content-Range"] = $"{entity} {(page - 1) * perPage}-{page * perPage}/{result.Total}";
        Response.Headers["Access-Control-Expose-Headers"] = "Content-Range";

        return Ok(result);
    }

    /// <summary>
    /// GET /api/v1/{entity}/{id}
    /// Get a single entity by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(string entity, Guid id)
    {
        if (await _metadataStore.GetAsync(entity) == null)
            return NotFound(new { message = $"Entity '{entity}' not found." });

        var result = await _crudService.GetByIdAsync(entity, id);
        if (result == null)
            return NotFound(new { message = $"{entity} with id '{id}' not found." });

        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/{entity}
    /// Create a new entity.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(string entity, [FromBody] Dictionary<string, object?> data)
    {
        var metadata = await _metadataStore.GetAsync(entity);
        if (metadata == null)
            return NotFound(new { message = $"Entity '{entity}' not found." });

        // Validate with dynamic validator
        var validator = new DynamicEntityValidator(metadata);
        var validationResult = await validator.ValidateAsync(data);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        var result = await _crudService.CreateAsync(entity, data);
        return StatusCode(201, result);
    }

    /// <summary>
    /// PUT /api/v1/{entity}/{id}
    /// Update an existing entity.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string entity, Guid id, [FromBody] Dictionary<string, object?> data)
    {
        var metadata = await _metadataStore.GetAsync(entity);
        if (metadata == null)
            return NotFound(new { message = $"Entity '{entity}' not found." });

        var result = await _crudService.UpdateAsync(entity, id, data);
        if (result == null)
            return NotFound(new { message = $"{entity} with id '{id}' not found." });

        return Ok(result);
    }

    /// <summary>
    /// DELETE /api/v1/{entity}/{id}
    /// Delete an entity by ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string entity, Guid id)
    {
        if (await _metadataStore.GetAsync(entity) == null)
            return NotFound(new { message = $"Entity '{entity}' not found." });

        var deleted = await _crudService.DeleteAsync(entity, id);
        if (!deleted)
            return NotFound(new { message = $"{entity} with id '{id}' not found." });

        return Ok(new { message = $"Deleted successfully.", id });
    }
}
