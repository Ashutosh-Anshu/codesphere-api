using codesphere_api.Common.Models;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string Icon { get; set; } = string.Empty;

    public string? Route { get; set; } 

    public bool IsActive { get; set; } = true;

    // Self-referencing hierarchy
    public Guid? ParentId { get; set; }

    public Menu? Parent { get; set; }

    public ICollection<Menu> Children { get; set; } = [];

    // Permissions/actions available for this menu
    public ICollection<Permission> Permissions { get; set; } = [];
}
