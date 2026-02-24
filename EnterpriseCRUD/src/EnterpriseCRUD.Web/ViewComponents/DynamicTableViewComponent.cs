using EnterpriseCRUD.Domain.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRUD.Web.ViewComponents;

public class DynamicTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<object> data, EntityMetadata metadata)
    {
        var model = new DynamicTableViewModel
        {
            Data = data,
            Metadata = metadata
        };
        return View(model);
    }
}

public class DynamicTableViewModel
{
    public IEnumerable<object> Data { get; set; } = new List<object>();
    public EntityMetadata Metadata { get; set; } = null!;
}
