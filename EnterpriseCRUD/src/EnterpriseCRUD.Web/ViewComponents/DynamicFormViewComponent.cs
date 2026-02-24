using EnterpriseCRUD.Domain.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.Web.ViewComponents;

public class DynamicFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(EntityMetadata metadata, object? entity = null)
    {
        var model = new DynamicFormViewModel
        {
            Metadata = metadata,
            Entity = entity
        };
        return View(model);
    }
}

public class DynamicFormViewModel
{
    public EntityMetadata Metadata { get; set; } = null!;
    public object? Entity { get; set; }
}
