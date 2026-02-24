using EnterpriseCRUD.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.Web.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IMetadataStore _metadataStore;

    public SidebarViewComponent(IMetadataStore metadataStore)
    {
        _metadataStore = metadataStore;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var modules = await _metadataStore.GetAllAsync();
        return View(modules.Values
            .Where(m => m.IsVisible)
            .OrderBy(m => m.Group)
            .ThenBy(m => m.DisplayName));
    }
}
