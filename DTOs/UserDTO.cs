using codesphere_api.Common.Models;

namespace codesphere_api.DTOs
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public Guid? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class RoleDTO
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RoleMenuDTO
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public List<PermissionDTO> Permissions { get; set; }
    }
    public class PermissionDTO
    {
        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

    }

}
