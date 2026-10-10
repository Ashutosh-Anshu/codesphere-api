using codesphere_api.Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace codesphere_api.DTOs
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName ?? string.Empty}".Trim();

        public string Email { get; set; } = string.Empty;

        public Guid? RoleId { get; set; }
        [NotMapped]
        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [NotMapped]
        public bool IsSystem { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class RoleItemDTO
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
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
        public List<PermissionDTO> Permissions { get; set; } = new();
    }
    public class PermissionDTO
    {
        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

    }

    public class MenuPermissionDTO
    {
        public Guid MenuId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsAllowed { get; set; }
    }

    public class RoleDetailDTO : RoleDTO
    {
        public List<MenuPermissionDTO> Permissions { get; set; } = new();
    }

    public class UserDetailDTO : UserDTO
    {
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class UserDetailResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName ?? string.Empty}".Trim();
        public string Email { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public string Token { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }

        public virtual UserDetailResponse User { get; set; } = null!;
    }

    public class MenuDTO
    {
        public Guid MenuId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Route { get; set; }
        public Guid? ParentId { get; set; }

    }


}
