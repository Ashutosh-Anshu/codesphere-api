using codesphere_api.Common.DTOs;
using codesphere_api.DTOs;
using Microsoft.AspNetCore.Mvc;

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

        Task<ApiResponse<bool>> CreateOrUpdateRoleAsync(
            [FromBody] RoleDetailDTO roleDetail,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleDetailDTO?>> GetRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteRoleByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);
    }
}
