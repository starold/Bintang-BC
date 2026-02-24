using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Domain.Enums;

namespace EnterpriseCRUD.Domain.Entities;

public class Departments : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? HeadOfDepartmentId { get; set; }
}
