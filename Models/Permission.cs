using codesphere_api.Common.Models;
using codesphere_api.Models;

public class Permission : BaseEntity
{
    public Guid MenuId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public Menu Menu { get; set; } = default!;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
