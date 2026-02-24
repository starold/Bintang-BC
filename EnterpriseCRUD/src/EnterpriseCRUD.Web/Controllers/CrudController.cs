using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using EnterpriseCRUD.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.Web.Controllers;

/// <summary>
/// The core dynamic controller that handles all CRUD operations
/// for any entity registered in the metadata system.
/// </summary>
[Route("admin/{entityName}")]
[DynamicModuleAuthorize]
public class CrudController : Controller
{
    private readonly ICrudService _crudService;
    private readonly IMetadataStore _metadataStore;
    private readonly ILogger<CrudController> _logger;

    public CrudController(
        ICrudService crudService,
        IMetadataStore metadataStore,
        ILogger<CrudController> logger)
    {
        _crudService = crudService;
        _metadataStore = metadataStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string entityName, int page = 1, int pageSize = 25, string? sortField = null, string? sortOrder = null, string? search = null)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        var pagedResult = await _crudService.GetPagedAsync(entityName, page, pageSize, sortField, sortOrder, search, null);

        ViewBag.Metadata = metadata;
        return View(pagedResult);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string entityName)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        ViewBag.Metadata = metadata;
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(string entityName, [FromForm] Dictionary<string, string> formData)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        // Convert string values from form to object? for the service
        var data = formData.ToDictionary(k => k.Key, v => (object?)v.Value);

        try
        {
            await _crudService.CreateAsync(entityName, data);
            TempData["Success"] = $"{metadata.DisplayNameSingular} created successfully.";
            return RedirectToAction(nameof(Index), new { entityName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {EntityName}", entityName);
            ViewBag.Error = ex.Message;
            ViewBag.Metadata = metadata;
            return View();
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(string entityName, Guid id)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        var entity = await _crudService.GetByIdAsync(entityName, id);
        if (entity == null) return NotFound();

        ViewBag.Metadata = metadata;
        return View(entity);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(string entityName, Guid id, [FromForm] Dictionary<string, string> formData)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        var data = formData.ToDictionary(k => k.Key, v => (object?)v.Value);

        try
        {
            await _crudService.UpdateAsync(entityName, id, data);
            TempData["Success"] = $"{metadata.DisplayNameSingular} updated successfully.";
            return RedirectToAction(nameof(Index), new { entityName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityName} with Id {Id}", entityName, id);
            ViewBag.Error = ex.Message;
            ViewBag.Metadata = metadata;
            return View(await _crudService.GetByIdAsync(entityName, id));
        }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(string entityName, Guid id)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        try
        {
            var success = await _crudService.DeleteAsync(entityName, id);
            if (success) TempData["Success"] = $"{metadata.DisplayNameSingular} deleted successfully.";
            else TempData["Error"] = $"Failed to delete {metadata.DisplayNameSingular}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {EntityName} with Id {Id}", entityName, id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { entityName });
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(string entityName, string? search = null)
    {
        var metadata = await _metadataStore.GetAsync(entityName);
        if (metadata == null) return NotFound();

        // Get all data for export (no pagination)
        var pagedResult = await _crudService.GetPagedAsync(entityName, 1, 10000, null, null, search, null);
        
        var builder = new System.Text.StringBuilder();
        var fields = metadata.Fields.Where(f => f.ShowInList).ToList();

        // Header
        builder.AppendLine(string.Join(",", fields.Select(f => $"\"{f.Label}\"")));

        // Rows
        foreach (var item in pagedResult.Data)
        {
            var dict = item as IDictionary<string, object>;
            var row = fields.Select(f => {
                var camelKey = char.ToLowerInvariant(f.Name[0]) + f.Name[1..];
                var val = dict != null && dict.ContainsKey(camelKey) ? dict[camelKey] : "";
                return $"\"{val?.ToString()?.Replace("\"", "\"\"")}\"";
            });
            builder.AppendLine(string.Join(",", row));
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
        return File(bytes, "text/csv", $"{entityName}_export_{DateTime.Now:yyyyMMdd}.csv");
    }
}
