using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Domain.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.Web.Controllers;

public class SearchController : Controller
{
    private readonly ICrudService _crudService;
    private readonly IMetadataStore _metadataStore;

    public SearchController(ICrudService crudService, IMetadataStore metadataStore)
    {
        _crudService = crudService;
        _metadataStore = metadataStore;
    }

    [HttpGet]
    public async Task<IActionResult> Results(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return RedirectToAction("Index", "Home");

        var allModules = await _metadataStore.GetAllAsync();
        var searchResults = new List<GlobalSearchResult>();

        foreach (var module in allModules.Values)
        {
            // Search in each module (top 5 results each for global view)
            var pagedResult = await _crudService.GetPagedAsync(module.Name, 1, 5, null, null, query, null);
            
            if (pagedResult.Data.Any())
            {
                searchResults.Add(new GlobalSearchResult
                {
                    ModuleMetadata = module,
                    Items = pagedResult.Data
                });
            }
        }

        ViewBag.Query = query;
        return View(searchResults);
    }
}

public class GlobalSearchResult
{
    public EntityMetadata ModuleMetadata { get; set; } = null!;
    public IEnumerable<object> Items { get; set; } = new List<object>();
}
