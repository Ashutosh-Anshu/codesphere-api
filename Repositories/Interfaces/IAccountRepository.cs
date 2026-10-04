using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;

namespace codesphere_api.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<ApiResponse<PaginatedResponse<UserDTO>>> GetAllUserAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken);

        Task<ApiResponse<PaginatedResponse<RoleDTO>>> GetAllRoleAsync(
            QueryParameters queryParameters,
            CancellationToken cancellationToken);

        Task<ApiResponse<List<RoleMenuDTO>>> GetAllRoleMenu(
            CancellationToken cancellationToken);
    }
}
