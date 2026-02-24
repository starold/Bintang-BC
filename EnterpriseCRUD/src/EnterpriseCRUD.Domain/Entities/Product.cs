using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Domain.Enums;

namespace EnterpriseCRUD.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? Stock { get; set; }
}
