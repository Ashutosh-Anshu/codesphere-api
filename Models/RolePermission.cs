namespace codesphere_api.Models
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public ApplicationRole Role { get; set; } = default!;

        public Permission Permission { get; set; } = default!;
    }

}
